using System.Collections.Generic;

namespace Yam.Game.Scripts.Rhythm.Dev;

public interface IParsedTick
{
    float GetTime();
    float GetUCoord();
    bool IsComplex();
    bool TryContainTick(IParsedTick tick);
    List<IParsedTick> GetTickList();
}