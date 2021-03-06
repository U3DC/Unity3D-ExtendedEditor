#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TNRD.Editor {

    public static class DictionaryExtension {
        public static void AddOrReplace<T, T1>( this Dictionary<T, T1> dict, T key, T1 value ) {
            if ( dict.ContainsKey( key ) ) {
                dict[key] = value;
            } else {
                dict.Add( key, value );
            }
        }
    }

    public class ExtendedInput {

        private InputState state = new InputState();

        public Vector2 MouseDelta { get { return state.MouseDelta; } }
        public Vector2 DragDelta { get { return state.DragDelta; } }
        public Vector2 MousePosition { get { return Event.current.mousePosition; } }
        public Vector2 ScrollDelta { get { return state.ScrollDelta; } }

        public EventType Type { get { return Event.current.type; } }
        private EventType previousType;

        public void OnGUI() {
            if ( Event.current.type == EventType.Layout && previousType != EventType.Repaint ) {
                state.kPressedStates.Clear();
                state.mPressedStates.Clear();
            } else {
                state.Update();
            }

            previousType = Type;
        }

        public bool KeyDown( KeyCode k ) {
            if ( !Event.current.isKey ) return false;
            if ( state.Consumed ) return false;
            return state.GetValue( k );
        }

        public bool KeyUp( KeyCode k ) {
            if ( !Event.current.isKey ) return false;
            if ( state.Consumed ) return false;
            return !state.GetValue( k );
        }

        public bool KeyPressed( KeyCode k ) {
            if ( !Event.current.isKey ) return false;
            if ( state.Consumed ) return false;
            return state.kPressedStates.ContainsKey( k ) && state.kPressedStates[k];
        }

        public bool KeyReleased( KeyCode k ) {
            if ( !Event.current.isKey ) return false;
            if ( state.Consumed ) return false;
            return state.kPressedStates.ContainsKey( k ) && !state.kPressedStates[k];
        }

        public bool ButtonDown( EMouseButton b ) {
            if ( !Event.current.isMouse ) return false;
            if ( state.Consumed ) return false;
            return state.GetValue( b );
        }

        public bool ButtonUp( EMouseButton b ) {
            if ( !Event.current.isMouse ) return false;
            if ( state.Consumed ) return false;
            return !state.GetValue( b );
        }

        public bool ButtonPressed( EMouseButton b ) {
            if ( !Event.current.isMouse ) return false;
            if ( state.Consumed ) return false;
            return state.mPressedStates.ContainsKey( b ) && state.mPressedStates[b];
        }

        public bool ButtonReleased( EMouseButton b ) {
            if ( !Event.current.isMouse ) return false;
            if ( state.Consumed ) return false;
            return state.mPressedStates.ContainsKey( b ) && !state.mPressedStates[b];
        }

        public void Use() {
            state.Consumed = true;
        }

        private class InputState {

            public Dictionary<EMouseButton, bool> mouseStates = new Dictionary<EMouseButton, bool>();
            public Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>();

            public Dictionary<EMouseButton, bool> mPressedStates = new Dictionary<EMouseButton, bool>();
            public Dictionary<KeyCode, bool> kPressedStates = new Dictionary<KeyCode, bool>();

            public bool Consumed = false;

            public Vector2 MousePosition = new Vector2();
            public Vector2 MouseDelta = new Vector2();
            public Vector2 DragDelta = new Vector2();
            public Vector2 ScrollDelta = new Vector2();

            public void Update() {
                var evt = Event.current;

                ScrollDelta = Vector2.zero;
                if ( evt.isMouse ) {
                    MousePosition = evt.mousePosition;
                }

                switch ( evt.type ) {
                    case EventType.MouseDown:
                        if ( !mouseStates.ContainsKey( (EMouseButton)evt.button ) || !mouseStates[(EMouseButton)evt.button] ) {
                            mPressedStates.AddOrReplace( (EMouseButton)evt.button, true );
                        }
                        mouseStates.AddOrReplace( (EMouseButton)evt.button, true );
                        break;
                    case EventType.MouseUp:
                        if ( mouseStates.ContainsKey( (EMouseButton)evt.button ) ) {
                            if ( mouseStates[(EMouseButton)evt.button] ) {
                                mPressedStates.AddOrReplace( (EMouseButton)evt.button, false );
                            }
                        }
                        mouseStates.AddOrReplace( (EMouseButton)evt.button, false );
                        break;
                    case EventType.MouseMove:
                        MouseDelta = evt.delta;
                        break;
                    case EventType.MouseDrag:
                        mouseStates.AddOrReplace( (EMouseButton)evt.button, true );
                        DragDelta = evt.delta;
                        break;
                    case EventType.KeyDown:
                        if ( !keyStates.ContainsKey( evt.keyCode ) || !keyStates[evt.keyCode] ) {
                            kPressedStates.AddOrReplace( evt.keyCode, true );
                        }
                        keyStates.AddOrReplace( evt.keyCode, true );

                        if ( evt.shift ) {
                            keyStates.AddOrReplace( KeyCode.LeftShift, true );
                            keyStates.AddOrReplace( KeyCode.RightShift, true );
                        }
                        break;
                    case EventType.KeyUp:
                        if ( keyStates.ContainsKey( evt.keyCode ) ) {
                            if ( keyStates[evt.keyCode] ) {
                                kPressedStates.AddOrReplace( evt.keyCode, false );
                            }
                        }
                        keyStates.AddOrReplace( evt.keyCode, false );

                        if ( !evt.shift ) {
                            keyStates.AddOrReplace( KeyCode.LeftShift, false );
                            keyStates.AddOrReplace( KeyCode.RightShift, false );
                        }
                        break;
                    case EventType.ScrollWheel:
                        ScrollDelta = evt.delta;
                        break;
                    case EventType.DragUpdated:
                        break;
                    case EventType.DragPerform:
                        break;
                    case EventType.DragExited:
                        break;
                    case EventType.ContextClick:
                        mouseStates.AddOrReplace( EMouseButton.Right, false );
                        break;
                }
            }

            public bool GetValue( KeyCode k ) {
                if ( keyStates.ContainsKey( k ) ) {
                    return keyStates[k];
                } else {
                    return false;
                }
            }

            public bool GetValue( EMouseButton b ) {
                if ( mouseStates.ContainsKey( b ) ) {
                    return mouseStates[b];
                } else {
                    return false;
                }
            }

            private KeyCode[] MapCharacterToKeyCodes( char kchar ) {
                kchar = char.ToLower( kchar );
                switch ( kchar ) {
                    case ' ':
                        return new[] { KeyCode.Space };
                    case '1':
                        return new[] { KeyCode.Alpha1, KeyCode.Keypad1 };
                    case '2':
                        return new[] { KeyCode.Alpha2, KeyCode.Keypad2 };
                    case '3':
                        return new[] { KeyCode.Alpha3, KeyCode.Keypad3 };
                    case '4':
                        return new[] { KeyCode.Alpha4, KeyCode.Keypad4 };
                    case '5':
                        return new[] { KeyCode.Alpha5, KeyCode.Keypad5 };
                    case '6':
                        return new[] { KeyCode.Alpha6, KeyCode.Keypad6 };
                    case '7':
                        return new[] { KeyCode.Alpha7, KeyCode.Keypad7 };
                    case '8':
                        return new[] { KeyCode.Alpha8, KeyCode.Keypad8 };
                    case '9':
                        return new[] { KeyCode.Alpha9, KeyCode.Keypad9 };
                    case '0':
                        return new[] { KeyCode.Alpha0, KeyCode.Keypad0 };
                    case 'a':
                        return new[] { KeyCode.A };
                    case 'b':
                        return new[] { KeyCode.B };
                    case 'c':
                        return new[] { KeyCode.C };
                    case 'd':
                        return new[] { KeyCode.D };
                    case 'e':
                        return new[] { KeyCode.E };
                    case 'f':
                        return new[] { KeyCode.F };
                    case 'g':
                        return new[] { KeyCode.G };
                    case 'h':
                        return new[] { KeyCode.H };
                    case 'i':
                        return new[] { KeyCode.I };
                    case 'j':
                        return new[] { KeyCode.J };
                    case 'k':
                        return new[] { KeyCode.K };
                    case 'l':
                        return new[] { KeyCode.L };
                    case 'm':
                        return new[] { KeyCode.M };
                    case 'n':
                        return new[] { KeyCode.N };
                    case 'o':
                        return new[] { KeyCode.O };
                    case 'p':
                        return new[] { KeyCode.P };
                    case 'q':
                        return new[] { KeyCode.Q };
                    case 'r':
                        return new[] { KeyCode.R };
                    case 's':
                        return new[] { KeyCode.S };
                    case 't':
                        return new[] { KeyCode.T };
                    case 'u':
                        return new[] { KeyCode.U };
                    case 'v':
                        return new[] { KeyCode.V };
                    case 'w':
                        return new[] { KeyCode.W };
                    case 'x':
                        return new[] { KeyCode.X };
                    case 'y':
                        return new[] { KeyCode.Y };
                    case 'z':
                        return new[] { KeyCode.Z };
                    case '-':
                        return new[] { KeyCode.Minus, KeyCode.KeypadMinus };
                    case '=':
                        return new[] { KeyCode.Equals, KeyCode.KeypadEquals };
                    case '_':
                        return new[] { KeyCode.Underscore };
                    case '+':
                        return new[] { KeyCode.Plus, KeyCode.KeypadPlus };
                    case '[':
                        return new[] { KeyCode.LeftBracket };
                    case ']':
                        return new[] { KeyCode.RightBracket };
                    case ';':
                        return new[] { KeyCode.Semicolon };
                    case '\'':
                        return new[] { KeyCode.Quote };
                    case ',':
                        return new[] { KeyCode.Comma };
                    case '.':
                        return new[] { KeyCode.Period, KeyCode.KeypadPeriod };
                    case '/':
                        return new[] { KeyCode.Slash };
                    case '\\':
                        return new[] { KeyCode.Backslash };
                    case '`':
                        return new[] { KeyCode.BackQuote };
                    case ':':
                        return new[] { KeyCode.Colon };
                    case '"':
                        return new[] { KeyCode.DoubleQuote };
                    case '<':
                        return new[] { KeyCode.Less };
                    case '>':
                        return new[] { KeyCode.Greater };
                    case '?':
                        return new[] { KeyCode.Question };
                    case '\n':
                        return new[] { KeyCode.Return | KeyCode.KeypadEnter };
                    case '\t':
                        return new[] { KeyCode.Tab };
                    case '!':
                        return new[] { KeyCode.Exclaim };
                    case '@':
                        return new[] { KeyCode.At };
                    case '#':
                        return new[] { KeyCode.Hash };
                    case '$':
                        return new[] { KeyCode.Dollar };
                    case '^':
                        return new[] { KeyCode.Caret };
                    case '&':
                        return new[] { KeyCode.Ampersand };
                    case '*':
                        return new[] { KeyCode.Asterisk };
                    case '(':
                        return new[] { KeyCode.LeftParen };
                    case ')':
                        return new[] { KeyCode.RightParen };
                }

                return new KeyCode[0];
            }
        }
    }
}
#endif