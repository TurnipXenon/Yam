using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Test.Rhythm.Servers;

[TestSubject(typeof(ChartEditorVisualizer))]
public static class ChartEditorVisualizerTest
{
	public class CreateTicks
	{
		[Fact]
		public void ValidateTicks()
		{
			var host = new Mock<IRhythmGameHost>();
			host.Setup(h => h.GetStreamLength()).Returns(2.7f);

			var model = new ChartModel
			{
				ApproachRate = 5, // preempt time is 1.2f for approach rate of 5
			};
			model.TimingSections = new List<TimingSection>
			{
				new() { Timing = 1f, BPM = 60, BeatsPerMeter = 4 },
				new() { Timing = 2.1f, BPM = 80, BeatsPerMeter = 3 }
			};
			var chart = new Mock<ChartState>(model, host.Object);

			var resource = new Mock<IPooledNoteResource>();

			var visualizer = new ChartEditorVisualizer(host.Object, chart.Object, resource.Object);
			var tickStates = new List<NoteState>();
			var actualResults = visualizer.CreateTicksTask(tickStates);


			var expectedSections = new List<NoteState>
			{
				new() { Timing = 1f, Type = NoteType.Downbeat },
				new() { Timing = 1.25f },
				new() { Timing = 1.5f },
				new() { Timing = 1.75f },
				new() { Timing = 2f, Type = NoteType.Downbeat },
				new() { Timing = 2.1f, Type = NoteType.Downbeat },
				new() { Timing = 2.35f },
				new() { Timing = 2.6f },
			};
			Assert.Equal(expectedSections.Count, actualResults.Count);

			var index = 0;
			expectedSections.ForEach(ts =>
			{
				Assert.True(Math.Abs(ts.Timing - actualResults[index].Timing) < Single.Epsilon);
				index++;
			});
		}
	}
}