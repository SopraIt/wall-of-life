import React, { Component } from 'react';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Header from './Header';
import _ from 'underscore';
import classNames from 'classnames';
import Webcam from 'react-webcam';
import CommonConstants from '../constants/CommonConstants';
import InformationsActions from '../actions/InformationsActions';

import 'react-datepicker/dist/react-datepicker.css';

export default class User extends Component {
    constructor(props) {
        super(props);
        this.state = { selectedIcon: CommonConstants.userImages[0], nameInput: '', surnameInput: '', date: moment(), overlay: false, uploadedImage: false, file: {}, hand: 'r' };
        this.changeInputName = this.changeInputName.bind(this);
        this.changeInputSurname = this.changeInputSurname.bind(this);
        this.changeDate = this.changeDate.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.resetForm = this.resetForm.bind(this);
        this.captureImage = this.captureImage.bind(this);
        this.setRef = this.setRef.bind(this);
        this.fileChangedHandler = this.fileChangedHandler.bind(this);
        this.changeHand = this.changeHand.bind(this);
    }

    setRef(webcam) {
        this.webcam = webcam;
    }

    changeIcon(icon) {
        this.setState({ selectedIcon: icon });
    }

    handleSubmit() {
        const firstName = this.state.nameInput;
        const lastName = this.state.surnameInput;
        const dateOfBirth = this.state.date;
        const idUser = `${firstName}_${lastName}_${moment().format('HHmmss')}`;
        const user = {
            id: idUser,
            firstName,
            lastName,
            dateOfBirth: moment(dateOfBirth, 'DD/MM/YYYY'),
            age: moment().diff(dateOfBirth, 'years'),
            games: [],
            hand: this.state.hand,
            image: this.state.selectedIcon,
            level: 1
        };
        if (user.image.name === 'webcam') {
            InformationsActions.setWebcamImage({ url: user.image.url, name: idUser });
            user.image = { name: idUser, url: `${idUser}.jpg` };
        } else if (this.state.uploadedImage) {
            const extension = this.state.file.name.split('.')[1];
            InformationsActions.changeNameImage({ oldName: this.state.file.name, newName: idUser, extension });
            user.image = { name: idUser, url: `${idUser}.${extension}` };
        }
        InformationsActions.setUser(user);
    }

    resetForm() {
        this.setState({ surnameInput: '', nameInput: '', selectedIcon: CommonConstants.userImages[0], date: moment() });
    }

    changeInputName(event) {
        this.setState({ nameInput: event.target.value });
    }

    changeInputSurname(event) {
        this.setState({ surnameInput: event.target.value });
    }

    changeDate(date) {
        this.setState({ date });
    }

    openCloseOverlay(status) {
        this.setState({ overlay: status });
    }

    captureImage() {
        const imageSrc = this.webcam.getScreenshot();
        this.setState({ selectedIcon: { url: imageSrc, name: 'webcam', base64: true }, overlay: false });
    };

    fileChangedHandler(event) {
        const file = event.target.files[0];
        this.setState({ file, uploadedImage: true });
        InformationsActions.uploadImage(file);
    }

    changeHand() {
        this.setState({ hand: this.state.hand === 'r' ? 'l' : 'r' });
    }

    renderIcons() {
        const html = [];
        _.map(CommonConstants.userImages, (icon, index) => {
            return html.push(
                <button className="col-sm-6 animal" key={`iconUser_${index}`} onClick={() => this.changeIcon(icon)}>
                    <img className="imageAnimal " src={`images/${icon.url}`} alt={icon.name} />
                </button>
            )}
        );
        return html;
    }

    renderWebCamOverlay() {
        let html = null;
        const videoConstraints = {
            width: 290,
            height: 290,
            facingMode: 'user',
        };

        if (this.state.overlay) {
            html = (
                <div className="modal-dialog modal-dialog-centered modal-webcam text-center" role="document">
                    <div className="modal-content">
                        <div className="modalHeaderuser">
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span className="spanModalUser" aria-hidden="true" onClick={() => this.openCloseOverlay(false)}>×</span>
                            </button>
                            <h1 className="modal-title" id="exampleModalLongTitle">Quando sei pronto scatta la foto!</h1>
                        </div>
                        <Webcam
                            audio={false}
                            height={350}
                            ref={this.setRef}
                            screenshotFormat="image/jpeg"
                            width={350}
                            videoConstraints={videoConstraints}
                        />
                        <div className=" takePhoto">
                            <button type="button" className="button-photo button-photos takeShot" onClick={this.captureImage}>
                                <img className="img-button-create" src="images/photo-camera.png" />
                            </button>
                        </div>
                    </div>
                </div>
            );
        }

        return html;
    }

    render() {
        const overlayClass = classNames({
            'modal fade in': true,
            open: this.state.overlay,
        });

        return (
            <div>
                <Header title="Aggiungi utente"/>
                <div className="container container-user ">
                    <div className="container-input-user col-sm-4">
                        <div className="blocked-field">
                            <form>
                                <div className="field">
                                    <input className="nameUser " type="text" id="fullname" value={this.state.nameInput} onChange={this.changeInputName}/>
                                    <label htmlFor="fullname">Nome</label>
                                </div>
                                <div className="field">
                                    <input className="cognomeUser" type="text" id="cognome" value={this.state.surnameInput} onChange={this.changeInputSurname}/>
                                    <label htmlFor="cognome">Cognome</label>
                                </div>
                                <div className="field">
                                    <label>Data di nascita</label>
                                    <DatePicker className="dataUser userDetails-data" onChange={this.changeDate} selected={this.state.date} dateFormat="DD/MM/YYYY" />
                                </div>
                            </form>
                        </div>
                    </div>
                    <div className="container-image-user col-lg-6">
                        <div className="col-sm-2 col-md-2 col-lg-3 containerimageAnimal">
                            {this.renderIcons()}
                        </div>

                        <div className="col-sm-5 col-md-4 col-lg-6 containerWebCam">
                            <img id="profile" className="randomDefaultUserBig" src={`${this.state.selectedIcon.base64 ? '' : 'images/'}${this.state.selectedIcon.url}`} alt={this.state.selectedIcon.name} />
                            <button className="userDetails-hands" onClick={this.changeHand}>
                                <span className="showUser-dx">{this.state.hand === 'r' ? 'DX' : 'SX'}</span>
                            </button>
                        </div>

                        <div className="col-sm-2 col-lg-3 containerButtonWebcam">
                            <button className="button-photo button-photoz" onClick={() => this.openCloseOverlay(true)}>
                                <img className="img-button-create" src="images/photo-camera.png" />
                            </button>
                        </div>
                    </div>
                </div>
                <div className="checkingUser">
                    <button className="button-close-user" onClick={this.resetForm}>
                        <img className="close-button-user img-size-usercreat-generic" src="images/cancel.png" />
                    </button>
                    <button className="button-save-user" onClick={this.handleSubmit}>
                        <img className="save-button-user img-size-usercreat-generic" src="images/checked.png" />
                    </button>
                </div>

                <div className={overlayClass} id="webcam" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                    { this.renderWebCamOverlay() }
                </div>
            </div>
        );
    }
}
