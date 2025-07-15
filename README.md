# CodexAngryBird
Make Angry Bird  with Codex

## BallShooter Script

A simple script is included at `Assets/Scripts/BallShooter.cs`.
It spawns three balls at start and lets you shoot them one by one
towards the mouse click position.

### How to use
1. Create a sphere prefab with a `Rigidbody` component and assign it to `ballPrefab`.
2. Create an empty game object for the spawn position and assign it to `spawnPoint`.
3. Attach the `BallShooter` component to any object in the scene.
4. Play the scene and click to launch up to three balls.
