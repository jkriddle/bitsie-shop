// Implementation of observer pattern. Allows objects to send messages back to DOM

// Base object that all other objects should inherit from.
// Allows for property extension by using myObject.init({foo: "bar" })
var BaseObject = Class.extend({
    options: {},

    init: function (opts) {
        for (var i in opts) {
            this[i] = opts[i];
        }
    },

});

var Observable = BaseObject.extend({

    subscribers: [],

    init: function (opts) {
        this._super(opts);
    },

    // Add a callback to the queue
    subscribe: function (messageName, callback) {
        this.subscribers.push({ message: messageName, callback: callback });
    },

    test: function () { alert('t'); },

    // Remove callback from queue
    unsubscribe: function (callback) {
        var i = 0,
            len = this.subscribers.length;

        // Iterate through the array and if the callback is
        // found, remove it.
        for (; i < len; i++) {
            if (this.subscribers[i] === callback) {
                this.subscribers.splice(i, 1);
                // Once we've found it, we don't need to
                // continue, so just return.
                return;
            }
        }
    },

    // Notify observers of a specified event
    notify: function (messageName, data) {
        console.log('notifying: ' + messageName);
        for (var i in this.subscribers) {
            if (this.subscribers[i].message == messageName) this.subscribers[i].callback(data);
        }
    }
});