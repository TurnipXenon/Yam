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
	private static void AssertEqualNotes(NoteState expected, NoteState actual)
	{
		Assert.True(Math.Abs(expected.Timing - actual.Timing) < Single.Epsilon);
		Assert.Equal(expected.Type, actual.Type);
	}

	public class CreateNotes
	{
		[Fact]
		public void ValidateNotesInitialization()
		{
			var host = new Mock<IRhythmGameHost>();
			host.Setup(h => h.GetStreamLength()).Returns(2.7f);

			var model = new ChartModel
			{
				ApproachRate = 5, // preempt time is 1.2f for approach rate of 5
			};
			var timingSection1 = new TimingSection { Timing = 1f, BPM = 60, BeatsPerMeter = 4 };
			var timingSection2 = new TimingSection { Timing = 2.1f, BPM = 80, BeatsPerMeter = 3 };
			model.TimingSections = new List<TimingSection>
			{
				timingSection1,
				timingSection2
			};
			var chart = new Mock<ChartState>(model, host.Object);

			var resource = new Mock<IPooledNoteResource>();

			var visualizer = new ChartEditorVisualizer(host.Object, chart.Object, resource.Object);
			var tickStates = new List<NoteState>();
			var actualResults = visualizer.CreateNotesTask(tickStates);


			var expectedNotes = new List<NoteState>
			{
				new(timingSection1) { Timing = 1f, Type = NoteType.Downbeat },
				new(timingSection1) { Timing = 1.25f },
				new(timingSection1) { Timing = 1.5f },
				new(timingSection1) { Timing = 1.75f },
				new(timingSection1) { Timing = 2f, Type = NoteType.Downbeat },
				new(timingSection2) { Timing = 2.1f, Type = NoteType.Downbeat },
				new(timingSection2) { Timing = 2.35f },
				new(timingSection2) { Timing = 2.6f },
			};
			Assert.Equal(expectedNotes.Count, actualResults.Count);

			var index = 0;
			expectedNotes.ForEach(ts =>
			{
				AssertEqualNotes(ts, actualResults[index]);
				index++;
			});
		}
	}

	public class Tick
	{
		[Fact]
		public async Task SimulateTick()
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

			// await until we get a passing state
			for (int i = 0; i < 20 && !visualizer.IsReady; i++)
			{
				await Task.Delay(50);
			}

			Assert.True(visualizer.IsReady);

			host.Setup(h => h.GetPlaybackPosition()).Returns(0);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestNote(It.IsAny<NoteState>()), Times.Once);
			AssertEqualNotes(expectedNotes.Dequeue(), calledNotes.Dequeue());

			host.Setup(h => h.GetPlaybackPosition()).Returns(0.03f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestNote(It.IsAny<NoteState>()), Times.Once);

			host.Setup(h => h.GetPlaybackPosition()).Returns(1f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestNote(It.IsAny<NoteState>()), Times.Exactly(6));
			for (int i = 0; i < 5; i++)
			{
				AssertEqualNotes(expectedNotes.Dequeue(), calledNotes.Dequeue());
			}
			
			host.Setup(h => h.GetPlaybackPosition()).Returns(1.8f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestNote(It.IsAny<NoteState>()), Times.Exactly(8));
			for (int i = 0; i < 2; i++)
			{
				AssertEqualNotes(expectedNotes.Dequeue(), calledNotes.Dequeue());
			}
			
			host.Setup(h => h.GetPlaybackPosition()).Returns(1.8f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestNote(It.IsAny<NoteState>()), Times.Exactly(8));
		}
	}
}