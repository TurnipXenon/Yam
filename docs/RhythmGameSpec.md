# Overview

Mimic the gameplay for Theatrhythm Curtain Call: https://www.youtube.com/watch?v=HY1E5pH94bQ

## Work

- [x] Get audio for inspo
- [x] Design data structure
- [x] Implement attaching meta of the song with the data structure
- [x] Implement playing song via
- [ ] Figure out dynamic timing (base it on osu)
	- Try to figure out the beat of the song maybe through a website: https://tunebat.com/Analyzer
- [ ] Figure out how to chart the song
	- [ ] How to place a beat
	- [ ] How to replay the song
	- [ ] How to place two beats at once
	- [ ] How to place a slide
	- [ ] How to place a hold
	- [ ] How to place a hold and a curve
	- [ ] How to adjust the curve

## Chart Structure

Reference: https://osu.ppy.sh/wiki/en/Client/File_formats/osu_%28file_format%29

**ChartMeta**

- SongName: string
- AudioFilename: string
- TimingPoints
	- Time: integer = start of timing section
	- BeatLength: float = the duration of a beat in milliseconds
	- Meter: integer = amount of beats in a meter
- HitObjects
	- Timing: ulong = time to react to the object from the beginning of the song
	- Y: integer = location across the direction that the beats are moving
	- Type: integer enum = type of hit object, see **HitType**
	- Channel: integer = relevant to hold, tells which of the holds are connected
	- HoldCurvePriority?: boolean = if there are two hold channels active, prioritize the one with this property set as true
	- BezCurveStart: Vector<2D> = the position of the handle of the bezier curve from the start. If the beats are moving left to right, this would be the position to the right.
	- BezCurveEnd: Vector<2D> = the position of the handle of the bezier curve from the end. If the beats are moving left to right, this would be the position to the left.

_Note_:

- We might have issues with different screen resolution when it comes to the BezCurve

**HitType**

- -1 Unknown
- 0 Unmarked
- 1 Hit Circle
- 2 Slide
- 3 Hold Start
- 4 Hold Middle
- 5 Hold Release
- 6 Slide to Hold Release
