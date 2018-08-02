namespace Bank
{
	public interface IProperties
	{
		object GetProperty(string key, object fallback);

		void SetProperty(string key, object value);
	}
}