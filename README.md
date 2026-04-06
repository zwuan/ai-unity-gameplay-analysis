# When Correct Code Fails: Evaluating AI-Generated Gameplay Logic in Unity

## Topic Claim

After reading this piece, a practitioner will understand how AI-generated gameplay control logic in Unity works well enough to decide where and how to integrate it into the runtime system without making the mistake of placing logic in the wrong execution loop or relying on brittle equality checks that break gameplay behavior.

---

## Project Overview

This project evaluates how AI-generated gameplay logic behaves in a real-time Unity environment.

A roll-a-ball style prototype is used. The player collects colored pickups and can pass through walls only when the ball color matches the wall color.

The goal is to determine whether AI-generated code can be directly integrated into a physics-based system or requires human correction.

---

## What This Project Demonstrates

- AI-generated Unity gameplay script (player controller)
- A deliberate runtime failure case
- A corrected implementation based on design decisions
- A clearly documented Human Decision Node

---

## Failure Case

Two failures were observed in the AI-generated version:

### Physics Execution Error
- Movement force applied in `Update()`
- Result: frame-dependent acceleration and unstable control

### Color Matching Failure
- Color comparison implemented using `a == b`
- Result: visually identical colors fail to match at runtime

These failures show that code can be syntactically correct while still producing incorrect gameplay behavior.

---

## Corrected Version

The revised implementation:

- Uses `FixedUpdate()` for physics-based movement
- Uses tolerance-based color comparison instead of exact equality

This aligns system behavior with Unity’s execution model and player expectations.
