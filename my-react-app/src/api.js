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
    getAccounts: () => post({ action: 'mstGetAccounts' }),
    addAccount: (account) => post({ action: 'mstAddAccount', payload: account }),
    updateAccount: (account) => post({ action: 'mstUpdateAccount', payload: account }),
    deleteAccount: (account) => post({ action: 'mstDeleteAccount', payload: account }),

    // Category Actions
    getCategories: () => post({ action: 'mstGetCategories' }),
    addCategory: (category) => post({ action: 'mstAddCategory', payload: category }),
    updateCategory: (category) => post({ action: 'mstUpdateCategory', payload: category }),
    deleteCategory: (category) => post({ action: 'mstDeleteCategory', payload: category }),

    // Article Actions
    getArticles: () => post({ action: 'mstGetArticles' }),
    addArticle: (article) => post({ action: 'mstAddArticle', payload: article }),
    updateArticle: (article) => post({ action: 'mstUpdateArticle', payload: article }),
    deleteArticle: (article) => post({ action: 'mstDeleteArticle', payload: article }),
    
    // Post Article
    postArticle: (account) => post({ action: 'postArticle', payload: account }),

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
