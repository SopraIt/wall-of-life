This project was bootstrapped with [Create React App](https://github.com/facebookincubator/create-react-app).

## Table of Contents

- [Folder Structure](#folder-structure)
- [Available Scripts](#available-scripts)
  - [grunt build](#grunt-build)
  - [npm run electron-win](#npm-run-electron-win)
  - [npm run package-win](#npm-run-package-win)
- [Build Process](#build-process)

## Folder Structure
Ending structure after the project's build will be the following one:

```
release-build/
  web-portal-win32-ia32/
    ...
    Data/
      ...
      StreamingAssets/
        db/
        Images/
          login-profile/
        Prizes/
    wall-of-life.exe
    web-portal.exe
```

############
### Available Scripts in the project directory:

#### `grunt build`
Builds the so called `WebPortal`, generates static files that would be bundled into electron

############
### Available Scripts in the electron directory:

#### `npm run electron-mac`
#### `npm run electron-win`
Run the electron app without compiling, meant for test/debug
Respectively, for iOs and for Windows

#### `npm run package-mac`
#### `npm run package-win`
Compiles the electron app and generates the build inside the `release-build` folder
Respectively, for iOs and for Windows

############
### Dev Ambient
First of all, let's compile the `WebPortal`: to do so, run `grunt build`.
Run electron, inside `electron_application_builder` folder, with command `npm run electron`, to run in debug mode.

### Build Process
To compile for release, start with compiling the `WebPortal` with `grunt build`.
The `\public` folder will be created inside `electron_application_builder` folder.
After that, let's compile our electron app with `npm run package-win`, which will create the following structure:

```
release-build/
  web-portal-win32-ia32/
    ...
    web-portal.exe
```
Next step is compiling the Unity application with the build process, choosing as destination folder the newly generated `web-portal-win32-ia32`.
This will be the current structure:

```
release-build/
  web-portal-win32-ia32/
    ...
    wall-of-life_Data/
      ...
      StreamingAssets/
        db/
        Images/
          login-profile/
        Prizes/
    wall-of-life.exe
    web-portal.exe
```

Rename the folder `wall-of-life_Data` into `Data`, so that the structure will be as the following one:

```
release-build/
  web-portal-win32-ia32/
    ...
    Data/
      ...
      StreamingAssets/
        db/
        Images/
          login-profile/
        Prizes/
    wall-of-life.exe
    web-portal.exe
```

Now all you have to do is copying/moving/renaming this folder and start Unity or the WebApp!
