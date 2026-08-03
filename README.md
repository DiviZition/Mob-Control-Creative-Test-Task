That's a test task for the Voodoo company for the Marketing developer position.

The project does not contain any DI solution, because I've decided not to add it to a "quick little prototype"... That was a mistake... All the references within a single scene are set up directly.
There is also no Bootstrapper yet; the architecture is pretty rough...

Notes:
1. All the game files lie inside Assets/_Core/...
2. There are 2 new physics layers: PlayerSideIgnoreSelf and EnemySideIgnoreSelf.
PlayerSideIgnoreSelf can't collide with itself. Same for EnemySideIgnoreSelf.
That means Player units never collide with each other. Same for Enemy units.

Here are the most important classes:

GameScenarioDirector:
The starting point is the GameScenarioDirector class, which is usually placed on the === SCENARIO === object on top of the hierarchy.
This class is enabling and disabling all the set-up active elements, and also finishes the game in case of the player's win or loss.
I thought it would be more helpful...

Damageable:
Base class for everything that can be damaged. It is a MonoBehaviour, so UnitBase, PlayerTower, EnemyTower, RoadBlocks, etc., are its heirs.
It has an "IsReturnDamage" property. If checked — when unit "Gregor" is taking damage from unit "Viktor", for example, "Gregor" will attack "Viktor" for the same amount of damage no matter if "Gregor" was killed by that first attack.
But that's a theory. The unit itself doesn't implement this logic. Instead, the attacking unit checks whether the unit it hits returns damage, and if yes — it applies the same damage to itself.

UnitDamageDealer:
The simple class that attacks all Damageables that stay within its trigger collider.
Also applies damage to itself in case the attacked Damageable had IsReturnsDamage == true.

BattleSide:
That's an enum with 2 types: Player, Enemy;
All the Damageables and some other scripts implement this property to define who to attack and who to ignore.

UnitSpawner:
It's a pool and the builder of a given unit prefab.

HordsUnitSpawner:
The non-MonoBehaviour class that is scheduling and constantly calling the UnitSpawner to spawn a configurable amount of units.
This class requires a lot of modifications, but it worked for this 5-day run.

UnitBase:
The base class all units inherit. It links the unit's components and performs the unit's death logic, enabling and disabling.
Enabling and disabling in terms of this class means respawning and going back into the pool.

UnitMovement:
It moves units via the NavMeshAgent manually. There is no path calculating.
Units are designed to always move in the direction they are looking at. But that doesn't always work like that.
To change this direction, UnitMovement provides a RotateUnit() method.
The direction a unit moves in changes only when calling RotateUnit() for better optimization.

UnitView:
Handles animations and some special logic about the unit's visual;

CanonShooter, CanonMovement, PlayerTower:
Those 3 classes are pretty simple. The only thing worth noting: PlayerTower enables and disables Canon scripts when the player is defeated, so they all work together only for now.

UnitMassiveSoundPlayer:
That's an experimental and actually not very stable sound handler.
When all units have their own AudioSource — the horde sounds really bad and loud. Unity mixers are also overloaded, so it's not the way I can make unit sounds.
This class handles only a few AudioSources, shaking them between units. It sounds much better, but has some new issues.
Units don't know about this class. This class connects to UnitSpawner and subscribes to unit events from there. This gives less control than I need.
It should be easily fixable with DI or an event bus.

PlayerTower, EnemyTower, and RoadBlock:
Those are just Damageables that fight back. We can subscribe to their OnDead or OnDamageTaken events to observe the game's state, play the hit effects, or update the HP counters.

MultiplyingGate:
Calls the UnitSpawner to spawn the required amount of units when a unit enters its trigger area.
The important thing is that the gate sets the just-spawned UnitBase's property "IgnoreGate" to itself. If we spawn a bunch of units in the gate's area — we'll constantly call UnitSpawner to spawn a new bunch of units.
That's a dead cycle, so the UnitBase has a link to the last gate it touched, and the MultiplyingGate ignores units with the link to itself.
The gates also have a method to increase the multiplying value. That's the way I've implemented player upgrades in the game.

GatesUpgradeHandler:
Simple script with a link to each MultiplyingGate. It has only one method: UpgradeAllGatesX().

GateUpgradePicker:
This is the upgrade object. When a player's unit enters the trigger — it calls GatesUpgradeHandler to upgrade all the gates by a given value. We can also choose if we are increasing or multiplying the X of the gates by our value.