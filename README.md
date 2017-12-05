# AbraxisToolset
The base toolset for the game, which includes a mod loader and a few other bits and pieces.

Feel free to support me and my projects on [my Patreon](https://www.patreon.com/zerndererer). I have no plans to continue development on this specific project, for now.

# Setup

# Modding
Normally, when adding new stuff, you won't need to patch the game, so it's a better idea to just create a mod.

**Step 1** : Make sure you've patched the game with something that loads mods (AbraxisToolset comes with a mod loader, for example)

**Step 2** : Make a new C# project, set it to build as a .dll (Class Library (.NET framework) for Visual Studio)

**Step 3** : Add the patched game as a reference ([Necropolis Folder]/Necropolis_Data/Managed/ is the folder you want to look for, Assembly-CSharp.dll is the Necropolis code, UnityEngine.dll is the Unity Engine, and the others are just whatever they are.)

**Step 4** : Create your mod from there! If you're using AbraxisToolset, any class that inherits from AbraxisToolset.ATMod has a few functions that call (Init will call before anything else, OnLoad will call once all other Init's have been called, Update will call every frame.)

**Step 5** : Build your mod once finished, and just copy it to the "mods" folder in Necropolis.

# Patching

# ***A FEW WARNINGS***

`0x0ade has said that technically patching will just keep the most recent patch, with other patches just being lost or something. In general, if you can, avoid making a patch for a game. Make mods to be loaded instead. I'll try to keep up with AbraxisToolset, and merge any changes that seem like they will improve it (Adding hooks that people want, adding new ways to interact with the default code, ect)`

**Now back to the tutorial**

I don't know EVERYTHING about patching, but I know enough to get by.
The first thing to note is that any class you put into the Assembly-CSharp.mm project will automatically be put into the game, even if you don't add any attributes to them.
To patch existing classes (viewed through [DNSpy](https://github.com/0xd4d/dnSpy) or the like), there's a few steps (5, 6, and 7).

**Step 1** : Fork/Clone the project. If you don't know how to do this, you're going to need to learn how to use Git, and this isn't the place for that.

**Step 2** : Copy "Assembly-CSharp.dll" and "UnityEngine.dll" from `[NecropolisFolder]/Necropolis_Data/Managed` to `lib-projs` in the project folder.

**Step 3** : **BACKUP "Assembly-CSharp.dll" AND "UnityEngine.dll"** If you don't back these up, you'll have to re-install the game to un-patch it.

**Step 4** : Open the project with whatever you want, I use Visual Studio 2017. You can use notepad if you like.

**Step 5** : Create a new class, and call it "patch_[Original Class Name]" (We're going to use the name "ExampleClass for this, so, patch_ExampleClass)
```
public class patch_ExampleClass {

}
```

**Step 6** : Add the MonoMod.MonoModPatch attribute to the class. You'll need to supply the original class' full name for the argument, too. Adding `global::` isn't required as far as I know, but it's explicit. And being explicit is a good idea.
```
[MonoMod.MonoModPatch("global:patch_ExampleClass")]
public class patch_ExampleClass {

}
```

**Step 7** : Patch/add methods/ect. 

This one's got a few sub-steps. If you want to patch over an existing method, you'll need to make a place for MonoMod to put the original function, unless you don't need it.
```
[MonoMod.MonoModPatch("global:patch_ExampleClass")]
public class patch_ExampleClass {
  
  public extern void orig_ExampleMethod();
  public void ExampleMethod(){
    //Do some code.
  }
}
```
This will tell MonoMod to put the original ExampleMethod code inside orig_ExampleMethod, and then put your code where it used to be. So now, in the game, whenever anything calls ExampleClass.ExampleMethod, it will call your code, not the original code.
If you need to call the original code for ExampleMethod, all you need to do is call orig_ExampleMethod.
If you want to add a new function, you don't need anything special, just make a new method like you normally would. MonoMod will patch it into the class.

If you need access to the variables or other methods from the original class, all you need to do is make the patch class inherit the original class like this : `public class patch_ExampleClass : ExampleClass`
Note that MonoMod will automatically fix references between patch and original classes. If you call patch_ExampleClass.ExampleMethod in your code somewhere, MonoMod will automatically replace that with ExampleClass.ExampleMethod.

**Step 8** : Once you're done with writing all your code, build it into a .dll (It should be called Assembly-CSharp.mm.dll)

**Step 9** : Patch the game (Either using the [AbraxisToolsetInstaller](https://github.com/NecropolisModding/AbraxisToolsetInstaller), or using MonoMod.exe

**If using AbraxisToolsetInstaller** : Run the installer, and just follow the instructions. When it asks if you want to to install AbraxisToolset or a custom Toolset, choose custom.

**If using MonoMod.exe** Build MonoMod, and go to the build folder. 

  **1**:Copy the Assembly-CSharp.dll from the Necropolis folder (or lib-projs, if you want) into the folder that has MonoMod.exe

  **2**:Copy the built patch (Assembly-CSharp.mm.dll) into the same folder.

  **3**:Run MonoMod.exe
