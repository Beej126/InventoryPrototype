/************************************************************************
 *** This file is generated. Don't edit manually.
 *** Source: InventoryPrototype\Models\ApiResponse.cs
 *** Template: TypewriterTemplates\ClassProjector.tst
 ************************************************************************/
/* tslint:disable */

import { meta } from '../helpers/meta'; 

export class ApiResponse<T> {
  public static readonly _className: string = 'ApiResponse';

  public data!: T;

  public success: boolean = false;

  public errorMessages: string[] = [];
}

