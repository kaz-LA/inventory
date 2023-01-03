﻿using Aine.Inventory.Core.Interfaces;

namespace Aine.Inventory.Api.Endpoints.ProductEndpoints;

public class CreateProductRequest : IProduct
{
  public string ProductNumber { get; set; } = default!;
  public string Name { get; set; } = default!;
  public string? Description { get; set; }
  public int? SubCategoryId { get; set; }
  public int? ModelId { get; set; }
  public string? Color { get; set; }
  public string? Size { get; set; }
  public string? SizeUnit { get; set; }
  public double? Weight { get; set; }
  public string? WeightUnit { get; set; }
  public string? Style { get; set; }
  public int? ReorderPoint { get; set; }
  public double? StandardCost { get; set; }
  public double? ListPrice { get; set; }
  
  bool IProduct.IsActive { get; }
  int IProduct.Id { get; }
}
