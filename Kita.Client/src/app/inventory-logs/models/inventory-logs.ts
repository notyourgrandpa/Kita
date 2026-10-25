export interface InventoryLog {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  changeType: number;
  remarks: string;
  date: Date;
  stock: number;
  isVoided: boolean;
}
