using System;

namespace OriginsSL.Modules.Subclasses.Misc;

public class PitchShifter
{
	#region Private Static Memebers
	private const int MaxFrameLength = 16000;
	private readonly float[] _gInFifo = new float[MaxFrameLength];
	private readonly float[] _gOutFifo = new float[MaxFrameLength];
	private readonly float[] _gFfTWorkSpace = new float[2 * MaxFrameLength];
	private readonly float[] _gLastPhase = new float[MaxFrameLength / 2 + 1];
	private readonly float[] _gSumPhase = new float[MaxFrameLength / 2 + 1];
	private readonly float[] _gOutputAccum = new float[2 * MaxFrameLength];
	private readonly float[] _gAnaFrequency = new float[MaxFrameLength];
	private readonly float[] _gAnaMagnitude = new float[MaxFrameLength];
	private readonly float[] _gSynFrequency = new float[MaxFrameLength];
	private readonly float[] _gSynMagnitude = new float[MaxFrameLength];
	private long _gRover;
	#endregion

	#region Public Static  Methods
	/// <summary>
	/// Pitch shifts the given audio data.
	/// </summary>
	/// <param name="pitchShift">The pitch shift.</param>
	/// <param name="numSampsToProcess">The number of samples to process.</param>
	/// <param name="sampleRate">The sample rate.</param>
	/// <param name="indata">The input data.</param>
	public void PitchShift(float pitchShift, long numSampsToProcess, float sampleRate, float[] indata)
	{
		PitchShift(pitchShift, numSampsToProcess, 2048, 10, sampleRate, indata);
	}

	private void PitchShift(float pitchShift, long numSampsToProcess, long fftFrameSize, long osamp, float sampleRate, float[] indata)
	{
		double magn, phase, tmp, window, real, imag;
		double freqPerBin, expct;
		long i, k, qpd, index, inFifoLatency, stepSize, fftFrameSize2;

		float[] outdata = indata;
		/* set up some handy variables */
		fftFrameSize2 = fftFrameSize / 2;
		stepSize = fftFrameSize / osamp;
		freqPerBin = sampleRate / (double) fftFrameSize;
		expct = 2.0 * Math.PI * stepSize / fftFrameSize;
		inFifoLatency = fftFrameSize - stepSize;
		if (_gRover == 0) _gRover = inFifoLatency;

		/* main processing loop */
		for (i = 0; i < numSampsToProcess; i++)
		{
			/* As long as we have not yet collected enough data just read in */
			_gInFifo[_gRover] = indata[i];
			outdata[i] = _gOutFifo[_gRover - inFifoLatency];
			_gRover++;

			/* now we have enough data for processing */
			if (_gRover >= fftFrameSize)
			{
				_gRover = inFifoLatency;

				/* do windowing and re,im interleave */
				for (k = 0; k < fftFrameSize; k++)
				{
					window = -.5 * Math.Cos(2.0 * Math.PI * k / fftFrameSize) + .5;
					_gFfTWorkSpace[2 * k] = (float) (_gInFifo[k] * window);
					_gFfTWorkSpace[2 * k + 1] = 0.0F;
				}

				/* ***************** ANALYSIS ******************* */
				/* do transform */
				ShortTimeFourierTransform(_gFfTWorkSpace, fftFrameSize, -1);

				/* this is the analysis step */
				for (k = 0; k <= fftFrameSize2; k++)
				{
					/* de-interlace FFT buffer */
					real = _gFfTWorkSpace[2 * k];
					imag = _gFfTWorkSpace[2 * k + 1];

					/* compute magnitude and phase */
					magn = 2.0 * Math.Sqrt(real * real + imag * imag);
					phase = Math.Atan2(imag, real);

					/* compute phase difference */
					tmp = phase - _gLastPhase[k];
					_gLastPhase[k] = (float) phase;

					/* subtract expected phase difference */
					tmp -= k * expct;

					/* map delta phase into +/- Pi interval */
					qpd = (long) (tmp / Math.PI);
					if (qpd >= 0) qpd += qpd & 1;
					else qpd -= qpd & 1;
					tmp -= Math.PI * qpd;

					/* get deviation from bin frequency from the +/- Pi interval */
					tmp = osamp * tmp / (2.0 * Math.PI);

					/* compute the k-th partials' true frequency */
					tmp = k * freqPerBin + tmp * freqPerBin;

					/* store magnitude and true frequency in analysis arrays */
					_gAnaMagnitude[k] = (float) magn;
					_gAnaFrequency[k] = (float) tmp;

				}

				/* ***************** PROCESSING ******************* */
				/* this does the actual pitch shifting */
				for (int zero = 0; zero < fftFrameSize; zero++)
				{
					_gSynMagnitude[zero] = 0;
					_gSynFrequency[zero] = 0;
				}

				for (k = 0; k <= fftFrameSize2; k++)
				{
					index = (long) (k * pitchShift);
					if (index <= fftFrameSize2)
					{
						_gSynMagnitude[index] += _gAnaMagnitude[k];
						_gSynFrequency[index] = _gAnaFrequency[k] * pitchShift;
					}
				}

				/* ***************** SYNTHESIS ******************* */
				/* this is the synthesis step */
				for (k = 0; k <= fftFrameSize2; k++)
				{
					/* get magnitude and true frequency from synthesis arrays */
					magn = _gSynMagnitude[k];
					tmp = _gSynFrequency[k];

					/* subtract bin mid frequency */
					tmp -= k * freqPerBin;

					/* get bin deviation from freq deviation */
					tmp /= freqPerBin;

					/* take osamp into account */
					tmp = 2.0 * Math.PI * tmp / osamp;

					/* add the overlap phase advance back in */
					tmp += k * expct;

					/* accumulate delta phase to get bin phase */
					_gSumPhase[k] += (float) tmp;
					phase = _gSumPhase[k];

					/* get real and imag part and re-interleave */
					_gFfTWorkSpace[2 * k] = (float) (magn * Math.Cos(phase));
					_gFfTWorkSpace[2 * k + 1] = (float) (magn * Math.Sin(phase));
				}

				/* zero negative frequencies */
				for (k = fftFrameSize + 2; k < 2 * fftFrameSize; k++) _gFfTWorkSpace[k] = 0.0F;

				/* do inverse transform */
				ShortTimeFourierTransform(_gFfTWorkSpace, fftFrameSize, 1);

				/* do windowing and add to output accumulator */
				for (k = 0; k < fftFrameSize; k++)
				{
					window = -.5 * Math.Cos(2.0 * Math.PI * k / fftFrameSize) + .5;
					_gOutputAccum[k] += (float) (2.0 * window * _gFfTWorkSpace[2 * k] / (fftFrameSize2 * osamp));
				}
				for (k = 0; k < stepSize; k++) _gOutFifo[k] = _gOutputAccum[k];

				/* shift accumulator */
				//memmove(gOutputAccum, gOutputAccum + stepSize, fftFrameSize * sizeof(float));
				for (k = 0; k < fftFrameSize; k++)
				{
					_gOutputAccum[k] = _gOutputAccum[k + stepSize];
				}

				/* move input FIFO */
				for (k = 0; k < inFifoLatency; k++) _gInFifo[k] = _gInFifo[k + stepSize];
			}
		}
	}
	#endregion

	#region Private Static Methods

	private static void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
	{
		float wr, wi, arg, temp;
		float tr, ti, ur, ui;
		long i, bitm, j, le, le2, k;

		for (i = 2; i < 2 * fftFrameSize - 2; i += 2)
		{
			for (bitm = 2, j = 0; bitm < 2 * fftFrameSize; bitm <<= 1)
			{
				if ((i & bitm) != 0) j++;
				j <<= 1;
			}
			if (i < j)
			{
				temp = fftBuffer[i];
				fftBuffer[i] = fftBuffer[j];
				fftBuffer[j] = temp;
				temp = fftBuffer[i + 1];
				fftBuffer[i + 1] = fftBuffer[j + 1];
				fftBuffer[j + 1] = temp;
			}
		}
		long max = (long) (Math.Log(fftFrameSize) / Math.Log(2.0) + .5);
		for (k = 0, le = 2; k < max; k++)
		{
			le <<= 1;
			le2 = le >> 1;
			ur = 1.0F;
			ui = 0.0F;
			arg = (float) Math.PI / (le2 >> 1);
			wr = (float) Math.Cos(arg);
			wi = (float) (sign * Math.Sin(arg));
			for (j = 0; j < le2; j += 2)
			{

				for (i = j; i < 2 * fftFrameSize; i += le)
				{
					tr = fftBuffer[i + le2] * ur - fftBuffer[i + le2 + 1] * ui;
					ti = fftBuffer[i + le2] * ui + fftBuffer[i + le2 + 1] * ur;
					fftBuffer[i + le2] = fftBuffer[i] - tr;
					fftBuffer[i + le2 + 1] = fftBuffer[i + 1] - ti;
					fftBuffer[i] += tr;
					fftBuffer[i + 1] += ti;

				}
				tr = ur * wr - ui * wi;
				ui = ur * wi + ui * wr;
				ur = tr;
			}
		}
	}
	#endregion
}