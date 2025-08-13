const isWebView = window.chrome && window.chrome.webview;

const post = (message) => {
    if (isWebView) {
        window.chrome.webview.postMessage(message);
    } else {
        console.log('Skipping postMessage (not in WebView2):', message);
    }
};

export const csharpApi = {
    // Account Actions
    getAccounts: () => post({ action: 'getAccounts' }),
    addAccount: (account) => post({ action: 'addAccount', payload: account }),
    updateAccount: (account) => post({ action: 'updateAccount', payload: account }),
    deleteAccount: (account) => post({ action: 'deleteAccount', payload: account }),

    // Category Actions
    getCategories: () => post({ action: 'getCategories' }),
    addCategory: (category) => post({ action: 'addCategory', payload: category }),
    updateCategory: (category) => post({ action: 'updateCategory', payload: category }),
    deleteCategory: (category) => post({ action: 'deleteCategory', payload: category }),

    // Article Actions
    getArticles: () => post({ action: 'getArticles' }),
    addArticle: (article) => post({ action: 'addArticle', payload: article }),
    updateArticle: (article) => post({ action: 'updateArticle', payload: article }),
    deleteArticle: (article) => post({ action: 'deleteArticle', payload: article }),
    
    // Event Listener Wrappers
    addEventListener: (event, handler) => {
        if (isWebView) {
            window.chrome.webview.addEventListener(event, handler);
        }
    },
    removeEventListener: (event, handler) => {
        if (isWebView) {
            window.chrome.webview.removeEventListener(event, handler);
        }
    }
};
