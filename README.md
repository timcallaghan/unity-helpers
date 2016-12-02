# README #

A bunch of Unity3D helper classes.

### How do I get set up? ###

This repository is organised into normal game object script components and editor scripts.

* Place normal game object scripts inside your Assets folder (or sub-folder)
* Place editor scripts inside your Assets->Editor folder (create it if it doesn't exist)

### Components ###


#### BoxColliderGenerator ####
This is an editor component that works in conjunction with the `AutoBoxCollider` script to compute the bounding box for all (sub-)meshes for a game object.
You use it as follows:

* Add the AutoBoxCollider component to your top-level game object that contains multiple meshes (and sub meshes)
* This will cause the editor to display two extra buttons in the property inspector for the game object
* Click `Add Bounding Box Collider` to compute the bounding box containing all meshes and then add a box collider around them
* Click `Remove Bounding Box Collider` to remove the box collider on the top-level game object

![AutoBoxColliderProperyInspector.png](https://bitbucket.org/repo/8EbMEG/images/1315955649-AutoBoxColliderProperyInspector.png)
![AutoBoxColliderExample.png](https://bitbucket.org/repo/8EbMEG/images/2964007558-AutoBoxColliderExample.png)