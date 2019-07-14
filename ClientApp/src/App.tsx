import * as React from 'react';
import '@progress/kendo-theme-bootstrap/dist/all.css';
import * as toastr  from 'toastr';

//https://dev.to/nodefiend/quick-start-guide-for-react-router-v4-using-create-react-app-4h7j
import { BrowserRouter, Route, Redirect } from 'react-router-dom';
import { NavMenuWithRouter } from './components/NavMenu';

import { Home } from './components/Home';
import { Inventory } from './components/Inventory';

//declared in globals.d.ts
//too bad it can't be initialized when it's declared so it could be immutable const
(window as any).baseUrl = `${location.protocol}//${location.host}`;
const routeBaseUrl = document.getElementsByTagName('base')[0].getAttribute('href') || '';

String.prototype.trimStart = function (trimChar: ' ' | ',' | '/'): string {
  //***by convention also removing any trailing whitespace
  const rgx = new RegExp('^\\s*' + trimChar, 'g');
  return this.replace(rgx, '');
}

String.prototype.trimEnd = function (trimChar: ' ' | ',' | '/'): string {
  //***by convention also removing any trailing whitespace
  const rgx = new RegExp(trimChar + '\\s*$', 'g');
  return this.replace(rgx, '');
}


toastr.optionsOverride = {
  "closeButton": true,
  "debug": false,
  "newestOnTop": true,
  "progressBar": true,
  "positionClass": "toast-top-right",
  "preventDuplicates": true,
  "onclick": null,
  "showDuration": "300",
  "hideDuration": "1000",
  "timeOut": "5000",
  "extendedTimeOut": "0",
  "showEasing": "swing",
  "hideEasing": "linear",
  "showMethod": "fadeIn",
  "hideMethod": "fadeOut"
};

class App extends React.Component {
  render() {
    return (
      <BrowserRouter basename={routeBaseUrl}>
        <div>
          <NavMenuWithRouter />
          <div style={{ padding: "0 20px 0" }}>
            <Route exact path="/" component={Home} />
            <Route exact path="/Inventory" component={Inventory} />
          </div>
        </div>
      </BrowserRouter>
    );
  }
}

export default App;
