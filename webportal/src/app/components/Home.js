import React, { Component } from "react";
import _ from 'underscore';
import HomeIcon from './HomeIcon';
import Header from './Header';
import CommonConstants from '../constants/CommonConstants';

export default class Home extends Component {
    renderIcons() {
        const html = [];
        _.map(CommonConstants.homeIcons, (icon, index) => html.push(<HomeIcon key={`icon_${index}`} icon={icon} />));

        return html;
    }

    render() {
        return (
            <div>
                <Header home />
                <div className="start-wall">
                    <div className="row margin-top-rowindex">
                        {this.renderIcons()}
                    </div>
                </div>
            </div>
        );
    }
}
