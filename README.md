# CodexAngryBird
[![Developing a 2.5D Angry Birds-style Unity game with OpenAI Codex.](https://i.ytimg.com/vi/thMHzO1EGAg/hqdefault.jpg?sqp=-oaymwFBCNACELwBSFryq4qpAzMIARUAAIhCGAHYAQHiAQoIGBACGAY4AUAB8AEB-AH-CYACmgWKAgwIABABGGUgRihhMA8=&rs=AOn4CLDMPWgC2iKSsyYKwIhNg2ghRuxLAg)]([http://www.youtube.com/watch?v=thMHzO1EGAg](https://youtu.be/n_kJyiIcc7Q?si=1Dp2ahpdye56s1RO))

Make Angry Bird Style Game  with Codex
No Coding No GameDesign
Only use Unity Basic Objects and Compenet and Box Image

## BallShooter Script

A simple script is included at `Assets/Scripts/BallShooter.cs`.
It spawns three balls at start and lets you shoot them one by one
towards the mouse click position.

### How to use
1. Create a sphere prefab with a `Rigidbody` component and assign it to `ballPrefab`.
2. Create an empty game object for the spawn position and assign it to `spawnPoint`.
3. Attach the `BallShooter` component to any object in the scene.
4. Play the scene and click to launch up to three balls.

## BirdLauncher and DestroyOnImpact

The project also includes a minimal setup for an Angry Birds style game using the new scripts:

- `BirdLauncher.cs` – handles dragging and launching a bird from a slingshot position.
- `DestroyOnImpact.cs` – destroys an object when the collision impact exceeds a threshold.

### Basic scene setup
1. Create a new empty GameObject at the slingshot position and assign it as `launchPosition` on a `BirdLauncher` component.
2. Create a sphere prefab with a `Rigidbody` and assign it as `birdPrefab` on the `BirdLauncher`.
3. Add a `LineRenderer` component to the same object and assign it to the `directionLine` field of `BirdLauncher` to display the predicted trajectory while dragging.
4. Build the stage using cubes, cylinders or spheres with `Rigidbody` components for blocks and pigs.
5. Attach `DestroyOnImpact` to any object that should break on strong collision (e.g. pigs or blocks).
6. Press Play and drag the mouse to aim. A line shows the expected path, release to launch a bird.

With only Unity's primitive objects and these two scripts you can quickly prototype a simple 3D Angry Birds style experience.
