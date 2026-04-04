public class ElfStateMachine
{
    public ElfState currentState;

    public void Initialize(ElfState startState) {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(ElfState newState) {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}