# Schema Terminology and Discussion

1. The rhythm game looks at **Charts**.
2. **Charts** are composed of metadata and a list of **Beats**.
3. A **Beat** is the smallest unit of player interaction with a combined singular judgment for all its parts. A **Beat** is *exclusively either*: (Hint to self: use oneOf https://stackoverflow.com/a/25033301/17836168)
   1. A metadata about what it is so the game knows how to parse it AND a list of **Ticks**.
   2. A metadata about what it is AND properties in a **Tick** (Hint: use allOf https://stackoverflow.com/a/52579526/17836168)
4. A **Tick** is the smallest unit of player interaction. It's composed of the origin point, and an optional two anchor points for **Beats** that use Bézier curves.

**Types of Beat**

- **SINGLE**
  - We might not need this type but we're putting it here: **SLIDE**
- **HOLD** (general)
  - i dont know if we just need to merge the two types of hold together
  - more distinct types:
    - **HOLD**
      - linear hold
      - bezier hold (todo(turnip): think whether i need to separate these types??)
    - **HOLD_THEN_SLIDE**
      - same type as hold?
- Thoughts: Do we even need to explicitly type our data???

**We have two space representations:**

1. T-U Space where data points are prefixed with TU and points are called TUCoord
2. X-Y Space where data points are prefixed with XY and points are called XYCoord

**Design philosophy**

- We want to keep data representation (T-U data) simple, and put all the junk in the X-Y space because we use T-U representation when we're authoring Charts.
- Factors affecting the transformation between T-U points to X-Y point can happen in runtime, so maybe X-Y points are calculated on the spot?

**Plan**

- [x] Mock data
  - Stick to hit and hold types (based on osu data)
  - Think about bezier holds later (based on possible sketches based on existing osu data. check out the gameplay for Sunleth Waterscape???)
  - Ticks within (bezier) holds are represented as individual beats in the osu mock up file. We've assigned x=103 (1st property in the HitObjects) as these inert parts.
- [x] Parse data from beat-test.rhythm.json
  - Start with RhythmPlayer.cs and mirroring the data in that json. Don't look ahead too much
- [ ] Channels for parallel beats should be calculated on load (one during runtime) and you do some sort of logic where you group beats that overlap with each other

**How to figure out which beats to visualize**

- Each channel should have a top index
- Reminder: the reason why we have different channels is for parallel inputs, i.e. we have to press A and B together

**Design decision: how to figure out when to instantiate beats as Godot objects**

1. Create inactive objects during instantiation
2. Rely on pooled objects, and slowly figure out which beats to instantiate. This assumes that time moves forward and we don't ever need
3. Alt: approach 2 but our index makes sure we are in the correct window???
  - Consider when we're at the end of the song, we should check a time window early. Pre-empting??

Considerations: we might want a looping feature???

TODO: object pooling generic cause we can
