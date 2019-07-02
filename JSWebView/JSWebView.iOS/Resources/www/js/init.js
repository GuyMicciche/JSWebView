function evalCSharp() {
    window.location = 'myapp://custom?args=COMPLETE';
}

function evalCSharpArgs(a1, a2) {
    CustomJavaScript.notify(a1 + " and " + a2);
}