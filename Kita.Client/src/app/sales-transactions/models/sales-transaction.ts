import { SalesTransactionItem } from '../models/sales-transaction-item';
export interface SalesTransaction{
  id: number;
  totalAmount: number;
  paymentMethod: string;
  processedByUserId: number;
  processedByUserName: string;
  date: Date;
  customerId: number;
  items: SalesTransactionItem[];
}
