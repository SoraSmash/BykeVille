export interface Product {
    productId: number;
    name: string;
    productNumber: string;
    color?: string;
    standardCost: number;
    listPrice: number;
    size?: string;
    weight?: number;
    productCategoryId?: number;
    productModelId?: number;
    sellStartDate: string;
    sellEndDate?: string | null;
    discontinuedDate?: string | null;
    thumbNailPhoto?: string;
    thumbnailPhotoFileName?: string;
    rowguid: string;
    modifiedDate: string;
    productCategory?: [];
    productModel?: [];
    salesOrderDetails: [];
  }