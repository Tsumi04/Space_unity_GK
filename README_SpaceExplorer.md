# Space Explorer (Unity 2D)

This project contains prefabs, scenes, and scripts for a simple 2D game where you pilot a spaceship, collect stars, and avoid asteroids.

## How to Open
- Open Unity Hub, click "Open", and select this folder.
- Unity version: Use the version recorded in `ProjectSettings/ProjectVersion.txt`.

## Controls
- Arrow keys: Move the spaceship
- Space: Shoot laser

## Scenes
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Gameplay.unity`
- Create `Assets/Scenes/EndGame.unity` (instructions below)

## What’s Implemented
- Player movement and screen clamping (`Assets/Script/PlayerController.cs`)
- Player shooting (`Laser.prefab` + `Laser.cs`)
- Random asteroid movement (`Asteroid.cs`)
- Enemy ships and enemy lasers (`EnemyController.cs`, `EnemyLaser.cs`)
- Spawning asteroids, stars, and enemies (`SpawnManager.cs`)
- Scoring and game flow (`GameManager.cs`)
- Main menu navigation (`MainMenuManager.cs`)

## Minimal Setup Checklist
1) Gameplay scene
- Create an empty `GameManager` object and add `GameManager` component.
- UI: Add a Canvas with a TextMeshProUGUI for score display; set its text to `Score: 0` and assign it to `GameManager.scoreText`.
- Ensure the Player prefab in the scene has:
  - `Rigidbody2D` (Dynamic), `Collider2D`, `SpriteRenderer`
  - `PlayerController` with `laserPrefab` set to `Assets/Prefab/Laser.prefab` and `firePoint` (use a child transform or the player itself).
- Ensure tags:
  - Player GameObject: tag `Player`
  - Asteroid prefabs: tag `Asteroid`
  - Star prefabs: tag `Star`
- `SpawnManager` object:
  - Add `SpawnManager` component
  - Assign `asteroidPrefabs` array with `Assets/Prefab/Asteroid.prefab` (and any variants if you have them)
  - Assign `starPrefab` to `Assets/Prefab/Star.prefab`
  - Assign `enemyPrefab` to `Assets/Prefab/EnemyShip.prefab`

2) EndGame scene
- Create a new scene `EndGame.unity` under `Assets/Scenes/`.
- Add a Canvas (UI Scale Mode: Scale With Screen Size).
- Add a TextMeshProUGUI and set placeholder to `Final Score: 0`.
- Create an empty `EndGameManager` object and add `EndGameManager` component.
- Drag the TMP text into `EndGameManager.finalScoreText`.
- Add two Buttons:
  - Button 1 text: `Main Menu` → OnClick: `EndGameManager.BackToMainMenu()`
  - Button 2 text: `Quit` → OnClick: `EndGameManager.QuitGame()`

3) Build Settings
- Open File → Build Settings…
- Add Open Scenes: `MainMenu`, `Gameplay`, `EndGame` in that order.

## Scoring Rules
- Collecting a `Star` adds `+10` (configurable in `GameManager.pointsPerStar`).
- Shooting an `Asteroid` deducts `-5` (configurable in `GameManager.pointsLostPerAsteroid`).
- If the player collides with an `Asteroid` or enemy laser, the game ends and switches to `EndGame` scene, showing the final score.

## Notes
- If you want frictionless movement, set `Rigidbody2D` linear drag to 0 and disable gravity scale for space-like movement.
- Ensure all colliders are sized to fit sprites and the stars use `Is Trigger` on their Collider2D.

Enjoy!








