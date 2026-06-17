export default class Order {
  constructor(symbol, side, quantity, price) {
    this.symbol = symbol;
    this.side = side;
    this.quantity = quantity;
    this.price = price;
  }
}
