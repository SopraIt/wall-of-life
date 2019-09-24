import AppDispatcher from '../dispatcher/AppDispatcher';
import ActionsConstants from '../constants/ActionsConstants';

export default {
    getInformations() {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.GET_INFORMATIONS,
        });
    },
    searchAction(criteria) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SEARCH,
            criteria
        });
    },
    setUser(user) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SET_USER,
            user,
        });
    },
    deleteUser(idUser) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.DELETE_USER,
            idUser,
        });
    },
    getUser(user) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.GET_USER,
            user,
        });
    },
    setWebcamImage(image) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SET_IMAGE,
            image,
        });
    },
    uploadTrophy(trophy) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.UPLOAD_TROPHY,
            trophy,
        });
    },
    saveUrl(trophy) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SAVE_URL_TROPHY,
            trophy,
        });
    },
    uploadImage(image) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.UPLOAD_IMAGE,
            image,
        });
    },
    changeNameImage(file) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.RENAME_IMAGE,
            file,
        });
    },
    changeUserInformations(informations) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SET_USER_INFORMATIONS,
            informations,
        });
    },
    setStars(stars) {
        AppDispatcher.dispatch({
            actionType: ActionsConstants.SET_STARS,
            stars,
        });
    },
};
