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
    public bool IsPreparing { get; set; }
    
    public string CodeName { get; }
    
    public string Name { get; }
    
    public string Description { get; }

    public float StartDuration { get; }
    
    public void PrepareGameMode();

    public void StartGameMode();

    public void StopGameMode();
}