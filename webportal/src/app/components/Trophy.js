import React, { Component } from "react";
import classNames from 'classnames';
import Header from './Header';
import InformationsActions from '../actions/InformationsActions';
import InformationsStore from '../stores/InformationsStore';

export default class Trophy extends Component {
    constructor(props) {
        super(props);
        this.state = {
            trophy: InformationsStore.prizes(),
            inputUrl: '',
            inputUrlDescription: '',
            inputFolderFile: undefined,
            inputFolderDescription: '',
            prize: null,
            overlayUrl: false,
            overlayUrlError: false,
            overlayFolder: false,
            overlayFolderError: false,
            inputStarsLevel1: InformationsStore.stars()['1'],
            inputStarsLevel2: InformationsStore.stars()['2'],
            inputStarsLevel3: InformationsStore.stars()['3']
        };
        this.changeInputUrl = this.changeInputUrl.bind(this);
        this.changeInputUrlDescription = this.changeInputUrlDescription.bind(this);
        this.changeInputFolder = this.changeInputFolder.bind(this);
        this.changeInputFolderDescription = this.changeInputFolderDescription.bind(this);
        this.closeUrlModal = this.closeUrlModal.bind(this);
        this.closeFolderModal = this.closeFolderModal.bind(this);
        this.uploadUrlPrize = this.uploadUrlPrize.bind(this);
        this.uploadFile = this.uploadFile.bind(this);
        this.onInformationsLoaded = this.onInformationsLoaded.bind(this);
        this.inputStarsLevel1 = this.inputStarsLevel1.bind(this);
        this.inputStarsLevel2 = this.inputStarsLevel2.bind(this);
        this.inputStarsLevel3 = this.inputStarsLevel3.bind(this);
        this.saveStars = this.saveStars.bind(this);
    }

    componentWillMount() {
        InformationsStore.addInformationsListener(this.onInformationsLoaded);
        if (!InformationsStore.initialized()) {
            InformationsActions.getInformations();
        }
    }

    componentWillUnmount() {
        InformationsStore.removeInformationsListener(this.onInformationsLoaded);
    }

    onInformationsLoaded() {
        this.setState({
            trophy: InformationsStore.prizes(),
            inputStarsLevel1: InformationsStore.stars()['1'],
            inputStarsLevel2: InformationsStore.stars()['2'],
            inputStarsLevel3: InformationsStore.stars()['3']
        });
    }
    
    uploadUrlPrize() {
        const source = this.state.inputUrl;
        const description = this.state.inputUrlDescription;
        const id = this.state.activeTropy;
        const type = source.indexOf('youtube') >= 0 ? 'ytvideo' : 'browser';

        if (description.length > 0 && source.length > 0) {
            InformationsActions.saveUrl({ source, description, id, type });
            this.closeUrlModal();
        } else {
            this.setState({ overlayFolderError: true });
        }
    }
    
    uploadFile() {
        const id = this.state.activeTropy;
        const file = this.state.inputFolderFile;
        const description = this.state.inputFolderDescription;

        if (description.length > 0 && file) {
            InformationsActions.uploadTrophy({ id, file, description });
            this.closeFolderModal();
        } else {
            this.setState({ overlayUrlError: true });
        }
    }

    changeInputUrl(event) {
        this.setState({ inputUrl: event.target.value });
    }
    
    changeInputUrlDescription(event) {
        this.setState({ inputUrlDescription: event.target.value });
    }
    
    changeInputFolder(event) {
        this.setState({ inputFolderFile: event.target.files[0] });
    }
    
    changeInputFolderDescription(event) {
        this.setState({ inputFolderDescription: event.target.value });
    }
    
    inputStarsLevel1(event) {
        this.setState({ inputStarsLevel1: event.target.value });
    }

    inputStarsLevel2(event) {
        this.setState({ inputStarsLevel2: event.target.value });
    }

    inputStarsLevel3(event) {
        this.setState({ inputStarsLevel3: event.target.value });
    }

    openUrlModal(id) {
        this.setState({ activeTropy: id, overlayUrl: true });
    }
    
    openFolderModal(id) {
        this.setState({ activeTropy: id, overlayFolder: true });
    }
    
    closeUrlModal() {
        this.setState({ activeTropy: '', overlayUrl: false, inputUrl: '', inputUrlDescription: '', overlayUrlError: false });
    }
    
    closeFolderModal() {
        this.setState({ activeTropy: '', overlayFolder: false, inputFolderFile: undefined, inputFolderDescription: '', overlayFolderError: false });
    }

    saveStars() {
        const stars = { 1: this.state.inputStarsLevel1, 2: this.state.inputStarsLevel2, 3: this.state.inputStarsLevel3 }
        InformationsActions.setStars(stars);
    }

    renderPrizeImage(type) {
        let image = 'star.png';
        if (type.indexOf('image') >= 0) {
            image = 'photo-camera.png';
        } else if (type === 'ytvideo') {
            image = 'video-play.png';
        } else if (type.indexOf('video') >= 0) {
        } else if (type === 'browser') {
            image = 'browser.png';
        } else if (type.indexOf('video') >= 0) {
            image = 'televisions.png';
        } else if (type.indexOf('audio') >= 0) {
            image = 'radio-antenna.png';
        } else if (type.indexOf('application') >= 0) {
            image = 'lace.png';
        }

        return image;
    }

    renderPrizes() {
        const html = [];

        for (var i = 1; i <= 6; i++) {
            const inputName = i;
            const file = this.state.trophy[inputName - 1];
            html.push(
                <div className="col-sm-2" key={i}>
                    <div className="awards" >
                        <img className="img image-generic" src={`images/${this.renderPrizeImage(file.type)}`} />
                        <p> {file.description.length > 0 ? file.description : 'Carica premio'} </p>
                        <button className="btn btn-light col-sm-4 col-md-4 col-lg-5 button-left btn btn-primary" type="button" onClick={() => this.openUrlModal(inputName)}>
                            <img className="buttons-awards" src="images/location.png" />
                        </button>
                        <button className="btn btn-light col-sm-4 col-md-4 col-lg-5 margin-button-awards button-right btn btn-primary" type="button" onClick={() => this.openFolderModal(inputName)}>
                            <img className="buttons-awards" src="images/folder.png" />
                        </button>
                    </div>
                </div>
            );
        }

        return html;
    }

    render() {
        const fileConsolation = this.state.trophy[6];
        const activeTropy = this.state.activeTropy;
        const modalDescription = activeTropy ? this.state.trophy[activeTropy - 1].description : '';
        const modalSource = activeTropy ? this.state.trophy[activeTropy - 1].source : '';
        const overlayUrlClass = classNames({
            'modal fade in': true,
            open: this.state.overlayUrl,
        });
        const overlayUrlErrorClass = classNames({
            'form-text text-muted descriptionMuted': true,
            hide: !this.state.overlayUrlError,
        });
        const overlayFolderClass = classNames({
            'modal fade in': true,
            open: this.state.overlayFolder,
        });
        const overlayFolderErrorClass = classNames({
            'form-text text-muted descriptionMuted': true,
            hide: !this.state.overlayFolderError,
        });
        return (
            <div>
                <Header />
                <div className=" start">
                    <div className="row stars-point">
                        <h2 className="text-prize-consolation">Stelle necessarie</h2>
                        <div className="stars-field-container col-sm-3">
                            <span>Livello 1</span>
                            <input className="userDetails-data" name="1" type="number" value={this.state.inputStarsLevel1} onChange={this.inputStarsLevel1} />
                        </div>
                        <div className="stars-field-container  col-sm-3">
                            <span>Livello 2</span>
                            <input className="userDetails-data" name="2" type="number" value={this.state.inputStarsLevel2} onChange={this.inputStarsLevel2} />
                        </div>
                        <div className="stars-field-container col-sm-3">
                            <span>Livello 3</span>
                            <input className="userDetails-data" name="3" type="number" value={this.state.inputStarsLevel3} onChange={this.inputStarsLevel3} />
                        </div>
                        <div className="col-sm-1 col-sm-offset-1 stars-field-container userButtons">
                            <button className="userButton save" onClick={this.saveStars}>
                                <img className="save-button-user img-size-usercreat-generic" src="images/checked.png" />
                            </button>
                        </div>
                    </div>
                    <div className="row row-start-wall">
                        <h2 className="text-prize-consolation">Premi</h2>
                        {/* Start Premi normali */}
                        {this.renderPrizes()}
                        {/* End Premi normali */}
                        
                        {/* Start Premio consolazione */}
                        <h2 className="text-prize-consolation">Premio di Consolazione</h2>
                        <div className=" col-sm col-md-4 col-lg-2 col-sm-6 col-sm-offset-3 col-md-4 col-md-offset-4 col-lg-2  col-lg-offset-5">
                            <div className="awards consolated" >
                                <img src="images/medal.png" className="img image-generic" />
                                <p> {fileConsolation.description.length > 0 ? fileConsolation.description : 'Carica premio'} </p>
                                <button className="btn btn-light col-sm-4 col-md-4 col-lg-5 button-left btn btn-primary" type="button" onClick={() => this.openUrlModal('7')}>
                                    <img className="buttons-awards" src="images/location.png" />
                                </button>
                                <button className="btn btn-light col-sm-4 col-md-4 col-lg-5 margin-button-awards button-right btn btn-primary" type="button" onClick={() => this.openFolderModal('7')}>
                                    <img className="buttons-awards" src="images/folder.png" />
                                </button>
                            </div>
                        </div>
                        {/* End Premio consolazione */}
                    </div>
                </div> 

                {/* Start Modal For WEB */}
                <div className={overlayUrlClass}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <button type="button" className="close" onClick={this.closeUrlModal}>
                                    <span>&times;</span>
                                </button>
                                <h1 className="modal-title">Inserisci il link</h1>
                            </div>
                            <div className="modal-body">
                                <h2>Carica URL</h2>
                                <input className="modalCenterWebInputUpload" value={this.state.inputUrl || modalSource} onChange={this.changeInputUrl}/>
                                <h2>Descrizione file</h2>
                                <input className="modalCenterWebInputDescription" value={this.state.inputUrlDescription || modalDescription} onChange={this.changeInputUrlDescription} />
                            </div>
                            <small className={overlayUrlErrorClass}>
                                Riempire tutti i campi prima di proseguire
                            </small>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" onClick={this.uploadUrlPrize}>Salva</button>
                            </div>
                        </div>
                    </div>
                </div>
                {/* End Modal For WEB */}

                {/* Start Modal For Upload */}
                <div className={overlayFolderClass}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <button type="button" className="close" onClick={this.closeFolderModal}>
                                    <span>&times;</span>
                                </button>
                                <h1 className="modal-title">Inserisci il File </h1>
                            </div>
                            <div className="modal-body">
                                <h2>Carica file</h2>
                                <input type="file" className="ModalCenterFolderInputUpload modalCenterFolderInputUploadhidden" onChange={this.changeInputFolder} />
                                <input className="modalCenterFolderInputUploadshow" value={this.state.inputFolderFile ? this.state.inputFolderFile.name : modalSource} />
                                <h2>Descrizione file</h2>
                                <input className="ModalCenterFolderInputDescription" value={this.state.inputFolderDescription || modalDescription} onChange={this.changeInputFolderDescription} />
                            </div>
                            <small className={overlayFolderErrorClass}>
                                La descrizione non può essere vuota.
                            </small>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" onClick={this.uploadFile}>Salva</button>
                            </div>
                        </div>
                    </div>
                </div>
                {/* Start Modal For Upload */}

            </div>
        );
    }
}
