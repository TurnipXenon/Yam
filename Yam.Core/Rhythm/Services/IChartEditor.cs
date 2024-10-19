using System.Numerics;
using Yam.Core.Rhythm.Models.Base;

namespace Yam.Core.Rhythm.Services;

public interface IChartEditor
{
	public void Play(ChartModel? chartModel);
	public void OnRewind();
	public void SetCursorPosition(Vector2 cursorPosition);
}