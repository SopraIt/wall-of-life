import React, { Component } from "react";
import Home from './Home';
import Search from './Search';
import Trophy from './Trophy';
import User from './User';
import UserDetails from './UserDetails';
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import InformationsStore from '../stores/InformationsStore';
import InformationsActions from '../actions/InformationsActions';

export default class App extends Component {
    render() {
        return (
            <Router>
                <div>
                    <Switch>
                        <Route exact path="/" component={Home} />
                        <Route path="/search" component={Search} />
                        <Route path="/trophy" component={Trophy} />
                        <Route path="/user" component={User} />
                        <Route path="/:id" component={UserDetails} />
                    </Switch>
                </div>
            </Router>
        );
    }
}
