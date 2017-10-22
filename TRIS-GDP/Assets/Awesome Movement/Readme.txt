Introduction
==========================
You can use the 2D and 3D movement components to code your movement code for your characters, NPCs and in game objects much easier.
The components have a few set of useful methods which allow you to express movement related logic in a more high level manner.

Applications
==========================
This package will be mostly useful for games with classic movement code. side scrollers, tile based games and alike are good examples. Also in networked games which you can not afford a lot of CharacterControllers, It's a great help.

How it works
==========================
These components use casts instead of CharacterControllers or colliders to check for collisions before moving. This has several advantages and it's just like how older games and many new games do it.
The performance is much higher compared to characterControllers , however when you need character controllers, you can use them in combination with the movement components. 
The movement components don't rely on any colliders or rigidbodies themselves but if your GameObject has a rigidbody, moves will be done using it and in the case of Movement component which uses 3D physics, If the GameObject has a character controller, it will use CharacterController.SimpleMove to move. You can change it to CharacterController.Move if desired.
Generally you set the component's properties in editor and then at runtime use Move and SmoothMove methods to move your characters just like our example scenes. If you need collisions with other objects like coins, enemies and ... then you can attach colliders, rigidbodies or anything else to the GameObject but Movement components don't need them to work.

Properties
==========================
You can set the layers that collisions are checked with them, the shape and size of the shape of your moving object using the component properties.

Methods
==========================
The component has a set of CanMove methods which check if the object can move using a certain vector without any collisions or not.
There is a set of Move methods which use CanMove to perform checks and if there was no collisions then move to the location. 
Some CanMove methods take a safeMove vector by reference which they populate with a safe movement vector in case that doing the full motion requested by you causes collisions.
Let's say you want to move 1 meter toward z axis but after moving 0.8 meters you'll encounter a collision, these family of methods will fill the safeMove vector by (0,0,0.8). TryMove methods use this safe motion to perform movement.
SmoothMove family of methods execute a coroutine which performs your move over time. For example if you tell the method to move toward z axis 2 metters and give 0.5 as speed. It will go there after 4 seconds. They also take a checkCollision flag which if you set as true, It will use CanMove to check for collisions before moving, otherwise it will just move without any checks.

Editor features
==========================
Both of the components has an inspector button to calculate the size automatically for you if you desire, The way that it works is that it adds a collider to the GameObject and calculates size based on that and then removes the collider.
Also both of them draw gizmos to aid you in setting properties.

Future improvements
==========================
We would like to add more high level functionalities like jumping, isGrounded and alike to the package and also make the inspector more beautiful.
We are also highly open to feedback and suggestions from you.

Demos
==========================
There are two simple demos in the project. The maze like 2d example uses the 2d movement component and the pacman like example uses the 3d component.
The codes are simple and self explanatory. The 2D one specially is very simple but the 3D one handles input and movement in a special way.