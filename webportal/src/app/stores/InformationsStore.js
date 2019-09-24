import EventEmitter from 'events';
import _ from 'underscore';
import moment from 'moment';
import Defiant from 'defiant.js/dist/defiant.min.js';
import AppDispatcher from '../dispatcher/AppDispatcher';
import ActionsConstants from '../constants/ActionsConstants';
import CommonConstants from '../constants/CommonConstants';

const EVENTS = {
    GET_INFORMATIONS: 'GET_INFORMATIONS',
    GET_USER: 'GET_USER',
};

let initialized = false;
let informations = {};
let user = { image: {}, games: [], dateOfBirt: moment() };
const stars = { 1: 0, 2: 0, 3: 0 };
const prizes = [
    {
        id: 1,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 2,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 3,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 4,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 5,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 6,
        type: '',
        description: '',
        source: ''
    },
    {
        id: 'consolation',
        type: '',
        description: '',
        source: ''
    }
]
let originalPeople = []; //Need for search

const InformationsStore = Object.assign({}, EventEmitter.prototype, {
    initialized() {
        return initialized;
    },
    informations() {
        return informations;
    },
    people() {
        return informations.people || [];
    },
    prizes() {
        return informations.prizes || prizes;
    },
    stars() {
        return informations.stars || stars;
    },
    user() {
        return user;
    },
    emitInformations() {
        this.emit(EVENTS.GET_INFORMATIONS);
    },
    addInformationsListener(callback) {
        this.on(EVENTS.GET_INFORMATIONS, callback);
    },
    removeInformationsListener(callback) {
        this.removeListener(EVENTS.GET_INFORMATIONS, callback);
    },
    emitGetUser() {
        this.emit(EVENTS.GET_USER);
    },
    addGetUserListener(callback) {
        this.on(EVENTS.GET_USER, callback);
    },
    removeGetUserListener(callback) {
        this.removeListener(EVENTS.GET_USER, callback);
    },
});

const getInformations = (idUser) => {
    fetch(CommonConstants.dbUrl)
        .then(db => {
            return db.json();
        }).then(data => {
            informations = data;
            originalPeople = data.people;
            initialized = true;
            InformationsStore.emitInformations();
            if (idUser) getUser(idUser);
        }
    );
};

const setUser = (user) => {
    fetch(CommonConstants.dbPeopleUrl, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    }).then((user) => {
        return user.json();
    }).then(() => {
        location.href = '/search';
    });
};

const deleteUser = (idUser) => {
    fetch(`${CommonConstants.dbPeopleUrl}/${idUser}`, {
        method: "DELETE"
    }).then((user) => {
        return user.json();
    }).then(() => {
        getInformations();
    });
};

const setWebcamImage = (image) => {
    const imageName = image.name;
    fetch(CommonConstants.base64Image, {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ image: image.url, name: imageName })
    }).then((response) => {
        user = _.extend({}, user, { image: { name: imageName, url: `${imageName}.jpg` } });
        InformationsStore.emitGetUser();
    }).catch(function (error) {
        console.log('Looks like there was a problem: \n', error);
    });;
};

const uploadTrophy = (trophy) => {
    const data = new FormData();
    data.append('trophy', trophy.file);
    fetch(CommonConstants.uploadTrophy, {
        method: "POST",
        body: data
    }).then((response) => {
        changeTrophyInformations({ source: trophy.file.name, type: trophy.file.type, id: trophy.id, description: trophy.description })
    }).catch(function (error) {
        console.log('Looks like there was a problem: \n', error);
    });;
};

const uploadImage = (image) => {
    const data = new FormData();
    data.append('image', image);
    fetch(CommonConstants.uploadImage, {
        method: "POST",
        body: data
    }).then((response) => {
        console.log(response);
    }).catch(function (error) {
        console.log('Looks like there was a problem: \n', error);
    });;
};

const renameImage = (file) => {
    fetch(CommonConstants.renameImage, {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(file)
    }).then((response) => {
        console.log(response);
    }).catch(function (error) {
        console.log('Looks like there was a problem: \n', error);
    });;
};

const getUser = (userId) => {
    let users = [];
    if (informations.people) {
        user = _.find(informations.people, user => user.id == userId);
        InformationsStore.emitGetUser();
    } else {
        fetch(CommonConstants.dbUrl)
            .then(db => {
                return db.json();
            }).then(data => {
                user = _.find(data.people, user => user.id == userId);
                InformationsStore.emitGetUser();
            }
        );
    }
};

const changeUserInformations = (informations) => {
    const idUser = informations.id;
    fetch(`${CommonConstants.dbPeopleUrl}/${idUser}`, {
        method: "PATCH",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(informations)
    }).then((user) => {
        return user.json();
    }).then(() => {
        getInformations(idUser);
    });
};

const changeTrophyInformations = (prize) => {
    const idPrize = prize.id;
    fetch(`${CommonConstants.dbPrizeUrl}/${idPrize}`, {
        method: "PATCH",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(prize)
    }).then((prize) => {
        return prize.json();
    }).then(() => {
        getInformations();
    });
};

const setStars = (stars) => {
    fetch(CommonConstants.dbStarsUrl, {
        method: "PATCH",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(stars)
    }).then((stars) => {
        return stars.json();
    }).then(() => {
        getInformations();
    });
};

const search = (criteria) => {
    if (criteria.length <= 0) {
        informations.people = originalPeople;
    } else {
        informations.people = JSON.search(informations.people, `//*[contains(firstName, "${criteria}") or contains(lastName, "${criteria}")]`);
    }
    InformationsStore.emitInformations();
};

AppDispatcher.register((action) => {
    switch (action.actionType) {
        case ActionsConstants.GET_INFORMATIONS:
            getInformations(action.card);
            break;
        case ActionsConstants.SET_USER:
            setUser(action.user);
            break;
        case ActionsConstants.DELETE_USER:
            deleteUser(action.idUser);
            break;
        case ActionsConstants.GET_USER:
            getUser(action.user);
            break;
        case ActionsConstants.SET_IMAGE:
            setWebcamImage(action.image);
            break;
        case ActionsConstants.UPLOAD_TROPHY:
            uploadTrophy(action.trophy);
            break;
        case ActionsConstants.SAVE_URL_TROPHY:
            changeTrophyInformations(action.trophy);
            break;
        case ActionsConstants.UPLOAD_IMAGE:
            uploadImage(action.image);
            break;
        case ActionsConstants.RENAME_IMAGE:
            renameImage(action.file);
            break;
        case ActionsConstants.SET_USER_INFORMATIONS:
            changeUserInformations(action.informations);
            break;
        case ActionsConstants.SEARCH:
            search(action.criteria);
            break;
        case ActionsConstants.SET_STARS:
            setStars(action.stars);
            break;
        default:
            // no op
            break;
    }
});

export default InformationsStore;
