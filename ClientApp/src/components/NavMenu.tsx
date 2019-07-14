import * as React from 'react';
import { Menu, MenuItem, MenuItemLink, MenuItemArrow, MenuSelectEvent } from '@progress/kendo-react-layout';
import { withRouter, RouteComponentProps } from 'react-router';

//https://github.com/DefinitelyTyped/DefinitelyTyped/issues/17181#issuecomment-362378619
export class NavMenu extends React.Component<RouteComponentProps<{}>, {}> {

  // https://www.telerik.com/kendo-react-ui/components/layout/menu/routing/
  private onSelect = (event: MenuSelectEvent) => {
    if (event.item.data) {
      this.props.history.push(event.item.data);
    }
  }

  public render() {

    return (
      <Menu onSelect={this.onSelect} style={{position: "sticky", top: 0}} >
        <MenuItem data="/" text="Home" />
        <MenuItem data="/Inventory" text="Inventory" />
      </Menu>

    );
  }
}

// https://www.telerik.com/kendo-react-ui/components/layout/menu/routing/
export const NavMenuWithRouter = withRouter(NavMenu);
