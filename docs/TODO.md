# TODO

- [x] Detect hold and move
  - [x] Focus on figuring out if the next beat has that element
  - [x] ~~Figure out the y coordinate of the current beat at the given time~~ Not needed because we move our mouse instead
- [ ] Scores
- [ ] Implement detecting hold location
- [ ] Implement hold end slide
- [ ] document how to add a beat type: from beatentity to chartentity to chart to beat
- [ ] document weird way we convert degree to radian to fit how godot gives float to
- [x] Implement mouse controls
- [ ] Implement keyboard direction controls
- [ ] Future: implement gamepad control?? Split between button and directional
- [ ] Logger level
- [ ] Cool rotation lerping (find closest rotation and simplify rotation when it exceeds?)
- [x] Fix beat pooling bug
  - [ ] Regression test for Godot objects???

## Slide

**25-02-21 Fri:** We are trying to investigate why the hold beat is being disposed too early? It only happens when we dispose beats within a hold beat ongoing. Why are future hold beats or ongoing hold beats being disposed and which calls are calling dispose on hold beat. I also wonder if we could make regression tests in the future to test these out.
  - Idea 1: we can run a server that runs Godot instances. Maybe even parallel if we can give them variables. I found out you can run a specific scene like `C:/Users/.../Projects/Godot/Godot_v4.4-beta1_mono_win64/Godot_v4.4-beta1_mono_win64.exe --path "C:\Users\...\Projects\Godot\Yam\Yam.Game" "res://Scenes/Rhythm/RhythmPlayground/rhythm_playground_slide.tscn"`.
  - Idea 2: maybe we need to separate our visuals further with our logic. We want to be able to dump our inputs, times, and events into a json, and we can simulate that in our core package? We might want to refactor our game for that though.
  - Finally found the problem. We have our hold tick beats as single beats, so when they reach their destruction point, they just release their resources but don't emit details back. Or maybe vice versa, I feel sleepy so I'm not checking which is which. This might be fixed once we differentiate a single beat and a single tick beat being reused.
**25-02-03 Mon:** We want to parse the direction from the file. From BeatEntity to ChartEntity to Chart to Beat. We store our direction in degrees but convert it to radian when crossing from Entity to our Godot Model. After that, make the logic for slide with its implementation. Then make the direction affect the sprite's rotation. Make note of the degree to radian in our SchemaTerminology.md
