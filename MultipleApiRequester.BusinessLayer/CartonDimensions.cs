namespace MultipleApiRequester.BusinessLayer;

public class CartonDimensions
{
    public CartonDimensions(int length, int width, int height)
    {
        Length = length;
        Width = width;
        Height = height;
    }

    // assume that we use millimeters, so we can use integer
    public int Length { get; }
    public int Width { get; }
    public int Height { get; }
}
