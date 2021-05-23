# The King's Table
A new Cave Story editor, designed to work with everything that came before, and everything that ever could happen after.

Please excuse the mess, this editor is only *just* in a state where you can use it to edit all of CS, so it's very liable to crashing, and is missing a lot of features.

In the future there will be more of a description here, but for now, I'm assuming you're here to build this for yourself.

# Running

## Windows

Check the ~~releases~~ [Appveyor](https://ci.appveyor.com/api/projects/brayconn/thekingstable/artifacts/Release.zip) page and download that exe!

## Mac

It's the same exe, but you have to run it using wine.

In *theory* it should work with mono,
but the winforms implementation on macOS is limited to 32bit,
and hasn't been updated in a long time, so I can't recommend it.

## Linux

It's still the same exe, just run it using mono.

If you aren't using the `mono-complete` package,
make sure you at least have `mono-locale-extras`,
otherwise TKT will crash because it can't find Shift-JIS.

# Building
It's just a regular VS 2019 file, it should build pretty normally.

Current Target is .NET Framework 4.8, but in the future this may lower to 4.6, and add an option for building with .NET 5.

That said, it has a LOT of other projects you'll need to download too

- [LayeredPictureBox](https://github.com/Brayconn/LayeredPictureBox)
- [PixelModdingFramework](https://github.com/Brayconn/PixelModdingFramework)
- [PETools](https://github.com/Brayconn/PETools) 
  - Make sure you use my overhaul, and not the original broken/super old parent repo.
- [LocalizeableComponentModel](https://github.com/Brayconn/LocalizeableComponentModel)
- [WinFormsKeybinds](https://github.com/Brayconn/WinFormsKeybinds)

I hope to reduce the amount of libraries you need to download in the future.