// -----------------------------------------------------------------------
// <copyright file="CursedGamemode.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace CursedMod.Features.Wrappers.Server.Gamemodes;

public abstract class CursedGamemode
{
    public virtual string GamemodeName { get; }
    
    public virtual string GamemodeDescription { get; }
    
    public virtual void PrepareGamemode() { }
    
    public virtual void StopGamemode() { }
}