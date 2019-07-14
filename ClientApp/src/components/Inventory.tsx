import * as React from 'react';
import styles from './Inventory.module.scss';
import { Grid, GridColumn, GridRowClickEvent, GridItemChangeEvent, GridFilterChangeEvent } from '@progress/kendo-react-grid';
import { fetchApi } from '../helpers/fetchApi';
import { InventoryItem } from '../models/InventoryItem';
import { debounce } from 'lodash';


interface IInventoryState {
  items: InventoryItem[];
  gridHeight: string;
}

export class Inventory extends React.Component<{}, IInventoryState> {

  private gridContainer?: HTMLElement;
  private fixGridHeight = () => {
    if (!this.gridContainer) return;
    const { top } = this.gridContainer.getBoundingClientRect();
    const bottomMargin = 20;
    this.setState({ gridHeight: (window.innerHeight - top - bottomMargin) + 'px' });
  }

  constructor(props: any) {
    super(props);

    this.state = {
      items: [],
      gridHeight: '500px'
    }

  }

  public componentDidMount() {
    fetchApi<{}, InventoryItem[]>('Inventory').then(response => {
      this.setState({ items: response.data });
    });

    window.onresize = debounce(this.fixGridHeight, 200, { leading: true });
    this.fixGridHeight();
  }

  private txtProdId?: HTMLInputElement;
  private txtQty?: HTMLInputElement;
  private onSubmit = (e: any) => {
    e.preventDefault();
    const data = { productId: this.txtProdId!.value, quantity: Number.parseInt(this.txtQty!.value) } as InventoryItem;
    fetchApi<InventoryItem, InventoryItem[]>('Inventory', null, data, 'POST').then(response => {
      this.setState({ items: response.data });
    });
  }

  public render() {
    return (
      <div className={styles.inventoryForm}>
        <h1>Inventory Page</h1>
        <form >
          <input className="form-control" placeholder="Item Number" ref={(ref: any) => this.txtProdId = ref} />
          <input className="form-control" placeholder="Quantity" ref={(ref: any) => this.txtQty = ref}/>
          <button className="btn btn-primary" onClick={this.onSubmit}>Submit</button>
        </form>

        <section>
          <div ref={(ref: any) => this.gridContainer = ref}>
            <Grid data={this.state.items}
              style={{ height: this.state.gridHeight }}
              resizable={true}
              editField="isEditable"
              //onRowClick={this.rowClick}
              //onItemChange={this.itemChange}
              //filterable={true}
              //filter={this.state.filter}
              //onFilterChange={(e: GridFilterChangeEvent) => this.setState({ filter: e.filter })}
            >
              <GridColumn field="productId" />
              <GridColumn field="quantity" />
              <GridColumn field="lastUpdated" format="{0:yyyy-MM-dd hh:mm:ss}" />
            </Grid>
          </div>
        </section>

      </div>
        );
      }
}