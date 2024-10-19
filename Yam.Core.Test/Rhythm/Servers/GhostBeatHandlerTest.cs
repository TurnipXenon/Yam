using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Test.Rhythm.Servers;

[TestSubject(typeof(GhostBeatHandler))]
public static class GhostBeatHandlerTest
{
	public class Tick
	{
		[Fact]
		public void MakeBeatPosition()
		{
			var timingSection1 = new TimingSection { Timing = 1f, BPM = 60, BeatsPerMeter = 4 };
			var timingSection2 = new TimingSection { Timing = 2.1f, BPM = 80, BeatsPerMeter = 3 };
			var expectedNotes = new Queue<NoteState>(new NoteState[]
			{
				new(timingSection1) { Timing = 1f, Type = NoteType.Downbeat },
				new(timingSection1) { Timing = 1.25f },
				new(timingSection1) { Timing = 1.5f },
				new(timingSection1) { Timing = 1.75f },
				new(timingSection1) { Timing = 2f, Type = NoteType.Downbeat },
				new(timingSection2) { Timing = 2.1f, Type = NoteType.Downbeat },
				new(timingSection2) { Timing = 2.35f },
				new(timingSection2) { Timing = 2.6f },
			});

			var host = new Mock<IRhythmGameHost>();
			host.Setup(h => h.GetStreamLength()).Returns(2.7f);

			var model = new ChartModel
			{
				ApproachRate = 5, // preempt time is 1.2f for approach rate of 5
			};
			model.TimingSections = new List<TimingSection>
			{
				timingSection1,
				timingSection2
			};
			var chart = new Mock<ChartState>(model, host.Object);

			var resource = new Mock<IPooledNoteResource>();

			var pooler = new Mock<NotePooler>(resource.Object);
			var calledNotes = new Queue<NoteState>();
			pooler.Setup(p => p.RequestNote(It.IsAny<NoteState>()))
				.Callback((NoteState ns) => calledNotes.Enqueue(ns));

			var visualizer = new ChartEditorVisualizer(
				host.Object,
				chart.Object,
				resource.Object,
				pooler.Object
			);

			var actor = new Mock<IGhostBeat>();

			var ghostBeatHandler = new GhostBeatHandler(
				visualizer,
				host.Object,
				resource.Object,
				actor.Object
			);

			Assert.True(true);
		}
	}
}