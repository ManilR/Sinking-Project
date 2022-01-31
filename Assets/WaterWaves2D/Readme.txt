Water Waves 2D by Andrii Sudyn

Online manual: https://ax23w4.com/devlog/waterwaves2d

To create new Water Waves 2D object in your scene, click on GameObject 
menu in Unity window, then go to 2D Object and then click Water Waves 2D.
That's it, now you have an object that will react if other 2D physics 
bodies enter or exit its area.

How does it work?

It creates a 2D mesh with a trigger collider and buoyancy effector and 
keeps an array of points. On trigger enter and exit it applies forces 
to the points, calculates their new positions based on spring equations
and updates vertex positions of the mesh.

Configuration

[The shape of water] lets you choose between surface and circle.
The waves spread in different ways for each of those.

[Size/Diameter] lets you set the size of your object.

[Resolution] defines number of points to calculate. More points will
result in smoother water but more calculations per frame. Also keep in
mind that wave properties depend on how close the points are and as
you increase the number of points the waves will look smaller, which
can be adjusted in Wave Properties section.

[Line thickness] is the width of the surface line. Setting it to 0
phisically removes it.

[Line color], [Outer color], [Inner color] will let you set colors
for the surface line and the gradient for the water. The mesh is 
being colored using vertex colors.

[Wobble speed] [Wobble damping] [Spread speed] and [Spread damping]
adjust the behavior of the waves. These numbers are used in spring
equations to determine how fast the points will oscilate and how
they'll affect their neighbors.

[Random ripples] section will let you set [Frequency], [Size] and
[Force] of random ripples. 

[Audio] section allows you to remove, add or replace the sounds
the object produces on trigger enter and leave.



My twitter: @ax23w4
Email: andrii.sudyn@gmail.com
My other assets: https://ax23w4.com/devlog/assets/
My games: https://ax23w4.com/devlog/games/