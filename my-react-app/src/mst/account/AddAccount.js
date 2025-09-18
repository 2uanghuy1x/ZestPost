import React, { useState, useEffect, useMemo } from 'react';
import { csharpApi } from '../../api';
import './AddAccount.css';

const formatOptions = {
    'username|pass|privatekey': ['username', 'password', 'privatekey'],
    'username|pass': ['username', 'password'],
    'uid|pass|privatekey|cookie': ['uid', 'password', 'privatekey', 'cookies'],
    'cookie': ['cookies'],
    'uid|pass': ['uid', 'password']
};

function AddAccount({ onClose, onSaveSuccess }) {
    const [inputText, setInputText] = useState('');
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [selectedFormat, setSelectedFormat] = useState(Object.keys(formatOptions)[0]);
    const [selectedRow, setSelectedRow] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'categoriesData') {
                const accountCategories = message.payload.filter(c => c.type === 'account');
                setCategories(accountCategories);
                if (accountCategories.length > 0) {
                    setSelectedCategory(accountCategories[0].id);
                }
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getCategories(); // Fetch categories

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const parsedAccounts = useMemo(() => {
        if (!inputText.trim()) {
            return [];
        }

        const lines = inputText.trim().split('\n');
        const formatKeys = formatOptions[selectedFormat];

        return lines.map(line => {
            const parts = line.split('|');
            const account = {
                // Default values
                name: '',
                uid: '',
                password: '',
                cookies: '',
                email: '',
                passmail: '',
                phone: '',
                mailrecovery: '',
                privatekey: '',
                avatar: '',
                categoryId: selectedCategory
            };

            formatKeys.forEach((key, index) => {
                if (parts[index]) {
                     if (key === 'username') {
                        account.name = parts[index].trim();
                     } else {
                        account[key] = parts[index].trim();
                     }
                }
            });
             // If name is not set by username, try to set it from uid
            if (!account.name && account.uid) {
                account.name = account.uid;
            }


            return account;
        });
    }, [inputText, selectedFormat, selectedCategory]);

    const handleSave = () => {
        if (parsedAccounts.length === 0) {
            alert('Không có tài khoản nào để lưu.');
            return;
        }

        const accountsToSave = parsedAccounts.map(acc => ({
            Name: acc.name,
            Uid: acc.uid,
            Password: acc.password,
            Cookies: acc.cookies,
            Email: acc.email,
            Passmail: acc.passmail,
            Phone: acc.phone,
            Mailrecovery: acc.mailrecovery,
            PrivateKey: acc.privatekey,
            Avatar: acc.avatar,
            CategoryId: selectedCategory === '' ? null : selectedCategory,
        }));

        accountsToSave.forEach(accountPayload => {
            csharpApi.addAccount(accountPayload);
        });

        alert(`${parsedAccounts.length} tài khoản đã được thêm thành công.`);
        onSaveSuccess();
    };
    
    const handleClear = () => {
        setInputText('');
    };

    const handleDeleteRow = () => {
        if (selectedRow === null) {
            alert('Vui lòng chọn một dòng để xóa.');
            return;
        }
        const lines = inputText.trim().split('\n');
        lines.splice(selectedRow, 1);
        setInputText(lines.join('\n'));
        setSelectedRow(null); // Deselect row after deletion
    };

    const fetchCategories = () => {
        csharpApi.getCategories();
    };


    return (
        <div className="add-account-container">
            <div className="add-account-header">
                <h2>Thêm Tài Khoản</h2>
                <button className="close-btn" onClick={onClose}>&times;</button>
            </div>

            <div className="input-area">
                <textarea
                    placeholder="Vui lòng nhập tài khoản vào đây ..."
                    value={inputText}
                    onChange={(e) => setInputText(e.target.value)}
                />
            </div>

            <div className="controls-section">
                <div className="control-group">
                    <label>Chọn danh mục</label>
                    <button onClick={fetchCategories} className="refresh-btn">&#x21bb;</button>
                    <select value={selectedCategory} onChange={e => setSelectedCategory(e.target.value)}>
                        {categories.map(cat => (
                            <option key={cat.id} value={cat.id}>{cat.name}</option>
                        ))}
                    </select>
                </div>
                <div className="control-group">
                    <label>Chọn định dạng</label>
                    <select value={selectedFormat} onChange={e => setSelectedFormat(e.target.value)}>
                        {Object.keys(formatOptions).map(format => (
                            <option key={format} value={format}>{format}</option>
                        ))}
                    </select>
                </div>
                <div className="format-info">
                    <label>Định dạng nhập</label>
                    <input type="text" value={selectedFormat} readOnly />
                    {/* <a href="#" className="format-link">Xem hướng dẫn tại đây ...</a> */}
                </div>
                <div className="action-buttons">
                    <button className="btn btn-new" onClick={handleClear}>Làm mới</button>
                    <button className="btn btn-delete-row" onClick={handleDeleteRow}>Xóa dòng</button>
                    <button className="btn btn-save" onClick={handleSave}>Lưu lại</button>
                </div>
            </div>

            <div className="preview-table-container">
                <table className="preview-table">
                    <thead>
                        <tr>
                            <th>STT</th>
                            <th>Tên / UID</th>
                            <th>Mật khẩu</th>
                            <th>UID</th>
                            <th>Private Key</th>
                            <th>Cookie</th>
                        </tr>
                    </thead>
                    <tbody>
                        {parsedAccounts.map((acc, index) => (
                            <tr 
                                key={index} 
                                className={selectedRow === index ? 'selected' : ''}
                                onClick={() => setSelectedRow(index)}
                            >
                                <td>{index + 1}</td>
                                <td>{acc.name}</td>
                                <td>{acc.password}</td>
                                <td>{acc.uid}</td>
                                <td>{acc.privatekey}</td>
                                <td>{acc.cookies}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default AddAccount;
