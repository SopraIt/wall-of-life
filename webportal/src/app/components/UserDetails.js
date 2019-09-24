import React, { Component } from "react";
import moment from 'moment';
import DatePicker from 'react-datepicker';
import _ from 'underscore';
import classNames from 'classnames';
import Webcam from 'react-webcam';
import Header from './Header';
import UserDetailsMatches from './UserDetailsMatches';
import CommonConstants from '../constants/CommonConstants';
import InformationsStore from '../stores/InformationsStore';
import InformationsActions from '../actions/InformationsActions';

import 'react-datepicker/dist/react-datepicker.css';

export default class UserDetails extends Component {
    constructor(props) {
        super(props);
        this.state = { user: InformationsStore.user(), overlay: false, buttonsToSave: false };
        this.onInformationsLoaded = this.onInformationsLoaded.bind(this);
        this.changeHand = this.changeHand.bind(this);
		this.changeTutorial = this.changeTutorial.bind(this);
        this.changeInputName = this.changeInputName.bind(this);
        this.changeInputSurname = this.changeInputSurname.bind(this);
        this.changeDate = this.changeDate.bind(this);
        this.changeLevel = this.changeLevel.bind(this);
        this.changeIcon = this.changeIcon.bind(this);
        this.resetForm = this.resetForm.bind(this);
        this.captureImage = this.captureImage.bind(this);
        this.setRef = this.setRef.bind(this);
        this.saveInformations = this.saveInformations.bind(this);
    }

    setRef(webcam) {
        this.webcam = webcam;
    }

    componentWillMount() {
        InformationsStore.addGetUserListener(this.onInformationsLoaded);
        InformationsActions.getUser(this.props.match.params.id);
    }

    componentWillUnmount() {
        InformationsStore.removeGetUserListener(this.onInformationsLoaded);
    }

    onInformationsLoaded() {
        if (_.isEqual(this.state.user, InformationsStore.user())) {
            // this.renderImageSlot();
        } else {
            this.setState({ user: InformationsStore.user() });
        }
        console.log(InformationsStore.user());
    }

    changeHand() {
        const user = _.extend({}, this.state.user, { hand: this.state.user.hand === 'r' ? 'l' : 'r' });
        this.setState({ user, buttonsToSave: true });
    }

	changeTutorial() {
        const user = _.extend({}, this.state.user, { skipTutorial: this.state.user.skipTutorial === 'true' ? 'false' : 'true' });
		this.setState({ user, buttonsToSave: true });
    }

    changeInputName(event) {
        const user = _.extend({}, this.state.user, { firstName: event.target.value });
        this.setState({ user, buttonsToSave: true });
    }

    changeInputSurname(event) {
        const user = _.extend({}, this.state.user, { lastName: event.target.value });
        this.setState({ user, buttonsToSave: true });
    }

    changeDate(date) {
        const user = _.extend({}, this.state.user, { dateOfBirth: date, age: moment().diff(date, 'years') });
        this.setState({ user, buttonsToSave: true });
    }

    changeLevel(event) {
        const user = _.extend({}, this.state.user, { level: event.target.value >= 3 ? 3 : event.target.value });
        this.setState({ user, buttonsToSave: true });
    }

    openCloseOverlay(status) {
        this.setState({ overlay: status });
    }

    changeIcon(icon) {
        const user = _.extend({}, this.state.user, { image: icon });
        this.setState({ user, buttonsToSave: true });
        this.openCloseOverlay(false);
    }

    resetForm() {
        this.setState({ user: InformationsStore.user(), buttonsToSave: false });
    }

    saveInformations() {
        InformationsActions.changeUserInformations(this.state.user);
        this.setState({ buttonsToSave: false });
    }

    captureImage() {
        const imageSrc = { url: this.webcam.getScreenshot(), name: this.state.user.id };
        this.setState({ overlay: false, buttonsToSave: true });
        InformationsActions.setWebcamImage(imageSrc);

    };

    renderIcons() {
        const html = [];
        _.map(CommonConstants.userImages, (icon, index) => {
            return html.push(
                <button className="col-sm-3 animal" key={`iconUser_${index}`} onClick={() => this.changeIcon(icon)}>
                    <img className="imageAnimal " src={`images/${icon.url}`} alt={icon.name} />
                </button>
            )
        });
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
                <div className="modal-dialog modal-dialog-centered modal-webcam" role="document">
                    <div className="modal-content text-center">
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
                        <div className="col-sm-12 col-md-12 col-lg-12 container-imageAnimal-userDetails">
                            {this.renderIcons()}
                        </div>
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

    renderButtons() {
        let html = null;

        if (this.state.buttonsToSave) {
            html = (
                <div className="col-sm-12 col-lg-1 userButtons">
                    <button className="userButton" onClick={this.resetForm}>
                        <img className="close-button-user img-size-usercreat-generic" src="images/cancel.png" />
                    </button>
                    <button className="userButton save" onClick={this.saveInformations}>
                        <img className="save-button-user img-size-usercreat-generic" src="images/checked.png" />
                    </button>
                </div>
            );
        }

        return html;
    }

    renderImageSlot() {
        const user = this.state.user;
        const date = new Date();
        const timestamp = date.getTime();
        return (
            <div className="child-userDetailsEdit">
                <canvas id="myCanvas"></canvas>
                <img id="profiles" className="userDetailsEdit-child" src={`images/${user.image.url}?${timestamp}`} />
                <button className="userDetails-hands" onClick={this.changeHand}>
                    <span className="showUser-dx">{user.hand === 'r' ? 'DX' : 'SX'}</span>
                </button>
                <button className="userDetails-edit" onClick={() => this.openCloseOverlay(true)}>
                    <img className="userDetails-pencil" src="images/edit.png" />
                </button>
            </div>
        );
    }

    render() {
        const user = this.state.user;
        const overlayClass = classNames({
            'modal fade in': true,
            open: this.state.overlay,
        });
		const tutorialClass = classNames({
			'userDetails-data' : true,
			'noTutorial' : this.state.user.skipTutorial === 'true'
		});
        return (
            <div>
                <Header backUrl="/search" />
                <div className="container-userDetails">
                    <div className="container">
                        <div className="col-sm-4 col-lg-4 userDetailsEdit">
                            { this.renderImageSlot() }
                        </div>
                        <div className="col-sm-4 col-lg-4 userDetailsData">
                            <div className="field-userDetails">
                                <p className="user-compiled">Nome</p>
                                <input className="userDetails-data" type="text" value={user.firstName} onChange={this.changeInputName} />
                            </div>
                            <div className="field-userDetailsDetails">
                                <p className="user-compiled">Cognome</p>
                                <input className="userDetails-data" type="text" value={user.lastName} onChange={this.changeInputSurname} />
                            </div>
                            <div className="field-userDetails">
                                <p className="user-compiled">Data di Nascita</p>
                                <DatePicker className="dataUser userDetails-data" selected={moment(user.dateOfBirth)} dateFormat="DD/MM/YYYY" onChange={this.changeDate} />
                            </div>
                            <div className="field-userDetails">
                                <p className="user-compiled">Età</p>
                                <input className="userDetails-data" type="text" value={user.age} disabled />
                            </div>
						    <div className="field-userDetails">
                                <p className="user-compiled">Mostra Tutorial</p>
								<div className={tutorialClass} onClick={this.changeTutorial}>
									<p className="tutorialOption tutorialYes">Sì</p>
									<p className="tutorialOption tutorialNo">No</p>
								</div>
                            </div>
                        </div>
                        <div className="col-sm-4 col-lg-3 userDetailsLevel">
                            <div className="child-userDetailsLevel">
                                <div className="container-level-details">
                                    <img className="userDetailsLevel-content-img" src="images/star.png" />
                                </div>
                                <p className="userDetails-real-level">livello</p>
                                <input className="userDetails-content-number" type="number" max="3" value={user.level} onChange={this.changeLevel} />
                            </div>
                        </div>

                        { this.renderButtons() }

                    </div>
                </div>
                <div className={overlayClass} id="webcam" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                    {this.renderWebCamOverlay()}
                </div>
                <UserDetailsMatches games={user.games} />
            </div>
        );
    }
}
