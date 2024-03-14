public interface IState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public partial class StateMachine {
    IState currentState;

    public void ChangeToState(IState newState) {
        if (currentState != null) {
            currentState.Exit();
        }
        
        currentState = newState;
        currentState.Enter();
    }

    public void Process() {
        if (currentState != null) {
            currentState.Execute();
        }
    }
}
