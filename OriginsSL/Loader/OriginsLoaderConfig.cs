using System.Collections.Generic;

namespace OriginsSL.Loader;

public class OriginsLoaderConfig
{
    public List<string> DisabledModules { get; set; } = [];
    public bool DisruptedEnabled { get; set; } = false;
}