# TODO

- [ ] document how to add a beat type: from beatentity to chartentity to chart to beat
- [ ] document weird way we convert degree to radian to fit how godot gives float to
- [ ] Make ActSingle test
- [ ] Implement mouse controls
- [ ] Implement keyboard controls
- [ ] Future: implement gamepad control?? Split between button and directional
- [ ] Logger level

## Slide

**25-02-03 Mon:** We want to parse the direction from the file. From BeatEntity to ChartEntity to Chart to Beat. We store our direction in degrees but convert it to radian when crossing from Entity to our Godot Model. After that, make the logic for slide with its implementation. Then make the direction affect the sprite's rotation. Make note of the degree to radian in our SchemaTerminology.md
