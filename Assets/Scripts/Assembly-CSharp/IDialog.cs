public interface IDialog
{
	bool isBlocking { get; }

	bool isDone { get; }

	void Update();

	void Destroy();
}
