using System;
using Aine.Inventory.Api.Endpoints.ProductInventoryEndpoints;
using Aine.Inventory.Core.Interfaces;
using Aine.Inventory.Core.ProductPriceAggregate;
using Aine.Inventory.SharedKernel.Interfaces;
using FastEndpoints;

namespace Aine.Inventory.Api.Endpoints.ProductPriceEndpoints;

public class Create : Endpoint<CreateProductPriceRequest, IProductPrice>
{
  private readonly IPriceService _service;

  public Create(IPriceService service)
  {
    _service = service;
  }

  public override void Configure()
  {
    Post("/products/{ProductId}/prices");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
     CreateProductPriceRequest request,
     CancellationToken cancellationToken)
  {
    if (request.ProductId <= 0)
    {
      await SendStringAsync("Invalid or unspecified ProductId!", StatusCodes.Status400BadRequest);
      return;
    }

    request.ChangedBy = User.Identity?.Name;
    var result = await _service.CreatePriceAsync(request, cancellationToken);
    if (result.IsSuccess)
    {
      await SendAsync(result.Value, StatusCodes.Status201Created);
      return;
    }

    var error = result.Errors?.Any() == true ? string.Join(Environment.NewLine, result.Errors) : "An error has occurred!";
    await SendStringAsync(error, StatusCodes.Status400BadRequest);
  }
}