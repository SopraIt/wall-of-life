// Modules to control application life and create native browser window
const {app, BrowserWindow, protocol} = require('electron');

const path = require('path');
const url = require('url');

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow;
let baseDir = process.env.DEV_MODE ? './public' : './public';
let resourceDir = process.env.DEV_MODE ? '../../Assets/StreamingAssets' : './Data/StreamingAssets';

function runJsonServer () {
  const jsonServer = require('json-server');
  const server = jsonServer.create();
  const router = jsonServer.router(resourceDir+"/db/db.json");
  const middlewares = jsonServer.defaults();

  server.use(middlewares);
  server.use(router);
  server.listen(3000, () => {
    console.log('JSON Server is running');
  });
}
runJsonServer();

function runReactServer () {
  let path = require('path');
  let express = require('express');
  const fs = require('fs');
  const bodyParser = require('body-parser');
  const cors = require('cors');
  const fileUpload = require('express-fileupload');
  let app = express();

  app.use(express.static(path.join(__dirname, baseDir)));
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());
  app.use(cors());
  app.use(fileUpload());

  app.post('/save_base64', (req, res) => {
    const image = req.body.image;
    const name = req.body.name;
    const base64Image = image.split(';base64,').pop();

    fs.writeFile(resourceDir+'/Images/login-profile/' + name + '.jpg', base64Image, { encoding: 'base64' }, function (err) {
      fs.writeFile(baseDir + '/images/' + name + '.jpg', base64Image, { encoding: 'base64' }, function (err) {
        console.log('File application created');
        res.sendStatus(200);
      });
    });
  });

  app.post('/prize_upload', function (req, res) {
    let uploadFile = req.files.trophy;
    const fileName = uploadFile.name;
    uploadFile.mv(
      `${resourceDir}/Prizes/${fileName}`,
      function (err) {
        if (err) {
          return res.status(500).send(err);
        }
        console.log('File uploaded');
        res.sendStatus(200);
      }
    );
  });
  
  app.post('/upload_image', function (req, res) {
    let uploadFile = req.files.image;
    const fileName = uploadFile.name;
    uploadFile.mv(
      `${resourceDir}/login-profile/${fileName}`,
      function (err) {
        if (err) {
          return res.status(500).send(err);
        }
        console.log('File application uploaded');
      }
    );
  });
  
  app.post('/rename_image', function (req, res) {
    let uploadFile = req.body;
    const fileOldName = uploadFile.oldName;
    const fileNewName = uploadFile.newName;
    const fileExtension = uploadFile.extension;
    fs.rename(`${resourceDir}/login-profile/${fileOldName}`, `${resourceDir}/login-profile/${fileNewName}.${fileExtension}`, function (err) {
      if (err) console.log('ERROR: ' + err);
    });
  });

  app.get("/", function (req, res) {
    res.status(404).send("404");
    res.status(500).send({ error: 'something blew up' });

  });
  app.get(/.*/, function(req, res) {
      res.sendFile(path.join(__dirname, baseDir, 'index.html'));
  });

  app.listen(8080);
}

runReactServer();

function createWindow () {
  // Rewrite for local path
  // protocol.interceptFileProtocol('file', (request, callback) => {
  //   const url = request.url.substr(7)    /* all urls start with 'file://' */
  //   callback({ path: path.normalize(`${__dirname}/${url}`)})
  // }, (err) => {
  //   if (err) console.error('Failed to register protocol')
  // })

  // Create the browser window.
  mainWindow = new BrowserWindow({width: 1024, height: 850,
                                  webPreferences: {
                                    webSecurity: true
                                  }
                                });
  
  
  // and load the index.html of the app.
  const startUrl = "http://127.0.0.1:8080" /*|| url.format({
            pathname: path.join(__dirname, './public/index.html'),
            protocol: 'file:',
            slashes: true
        })*/;
  mainWindow.loadURL(startUrl);

  // Open the DevTools.
  if(process.env.DEV_MODE)
    mainWindow.webContents.openDevTools();

  // Emitted when the window is closed.
  mainWindow.on('closed', function () {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', createWindow);

// Quit when all windows are closed.
app.on('window-all-closed', function () {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', function () {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (mainWindow === null) {
    createWindow();
  }
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.
