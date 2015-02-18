using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Flags]
public enum BoosterType
{
    None,
    DestroyWholeRowColumn
}


public static class BoosterTypeUtilities
{
    public static bool ContainsDestroyWholeRowColumn(BoosterType bt)
    {
        return (bt & BoosterType.DestroyWholeRowColumn) == BoosterType.DestroyWholeRowColumn;
    }
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}
