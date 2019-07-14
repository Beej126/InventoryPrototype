import * as React from 'react';
import logo from './ReactLogo.svg';

export class Home extends React.Component {

  public render() {
    return (
      <div style={{
        textAlign: "center",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        fontSize: "calc(10px + 2vmin)",
        color: "white"
      }}>
        <form>
          <img src={logo} style={{ animation: "App-logo-spin infinite 20s linear", height: "40vmin" }} alt="logo" />
          <p>
            Edit <code>src/App.tsx</code> and save to reload.
          </p>
          <a
            className="App-link"
            href="https://reactjs.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            Learn React
          </a>
        </form>
      </div>
    );
  }
}