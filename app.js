require('dotenv').config();
var express = require('express');
var session = require('express-session');
var MySQLStore = require('express-mysql-session')(session);
var exphbs = require('express-handlebars');
var express_handlebars_sections = require('express-handlebars-sections');
var bodyParser = require('body-parser');
var path = require('path');
var HomeController = require('./controllers/HomeController');
var app = express();
app.engine('hbs', exphbs({
    defaultLayout: '_LayoutPublic',
    layoutsDir: 'views/Layout/',
    partialsDir: 'views/partials/',
    helpers: {
        section: express_handlebars_sections(),
        compareStatus: function (v1, v2, option) {
            if (v1 === v2) {
                return option.fn(this);
            }
            return option.inverse(this);
        }
    }
}));

app.set('view engine', 'hbs');
app.set("views", path.join(__dirname, 'views'));
app.use(express.static(path.join(__dirname, '/public')));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: false
}));

var sessionStore = new MySQLStore({
    host: process.env.DB_HOST,
    port: process.env.DB_PORT,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_DATABASE,
    createDatabaseTable: true,
    schema: {
        tableName: 'sessions',
        columnNames: {
            session_id: 'session_id',
            expires: 'expires',
            data: 'data'
        }
    }
});

app.use(session({
    key: 'session_cookie_name',
    secret: 'session_cookie_secret',
    store: sessionStore,
    resave: false,
    saveUninitialized: false
}));

app.use('/home',HomeController);
app.get('/',(req,res)=>{
    res.redirect('/home');
})

console.log('Starting app with config:', process.env)
app.listen(process.env.PORT, () => {
  console.log('Site running on port ' + process.env.PORT);
});