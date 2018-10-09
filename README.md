# Elemental-Cataclysm
Mobile Game

  Elemental Cataclysm is a moblie game I developed for Android. The main premise of the game is to take a group of monsters and use them to fight through a procedurally generated dungeon. The game's main mechanic is the elemental identity system. Many objects can have elemental identities, such as monsters, abilities and rooms. Different elemental identities can special interactions with each other.
For example, a monster with a fire elemental identity will take more damage from an ability with a water elemental identity. This lets the player strategize base on their understanding of the elemental identity system. Victory is achieved when the player is able to successfully battle their way through the dungeon and defeat the final boss.

  The source code uses a singleton design pattern to communicate data between different scripts. The Game Manager script holds the data for the entire game. The Game Manager script has a public static variable called "instance", which is a reference to itself. Being a public static variable, it can be called from any script in the project. This makes it easy to communicate data in the Game Manger script to any script in the project.       
