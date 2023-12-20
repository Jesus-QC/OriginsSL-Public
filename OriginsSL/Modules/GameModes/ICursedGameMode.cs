// -----------------------------------------------------------------------
// <copyright file="ICursedGameMode.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace OriginsSL.Modules.GameModes;

public interface ICursedGameMode
{
    public string GameModeName { get; }
    
    public string GameModeDescription { get; }

    public bool IsCustomLobbyEnabled { get; }
    
    public void PrepareGameMode();

    public void StopGameMode();
}