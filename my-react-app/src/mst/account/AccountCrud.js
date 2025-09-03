import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './AccountCrud.css';
import '../category/CategoryCrud.css';
import AddAccount from './AddAccount';

function AccountCrud() {
    const [accounts, setAccounts] = useState([]);
    const [allAccounts, setAllAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [isAddAccountOpen, setIsAddAccountOpen] = useState(false);
    const [currentAccount, setCurrentAccount] = useState(null);

    const fetchAccounts = () => {
        csharpApi.getAccounts();
    };

    const fetchCategories = () => {
        csharpApi.getCategories();
    };

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
                setAllAccounts(message.payload);
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'actionSuccess') {
                setIsEditModalOpen(false);
                setCurrentAccount(null);
                fetchAccounts(); 
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        fetchAccounts();
        fetchCategories();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);
    
    const accountCategories = categories.filter(c => c.type === 'account');

    useEffect(() => {
        if (selectedCategory) {
            const filtered = allAccounts.filter(account => account.categoryId === selectedCategory);
            setAccounts(filtered);
        } else {
            setAccounts(allAccounts);
        }
    }, [selectedCategory, allAccounts]);

    const handleAddNew = () => {
        setIsAddAccountOpen(true);
    };

    const handleEdit = (account) => {
        setCurrentAccount(account);
        setIsEditModalOpen(true);
    };

    const handleDelete = (account) => {
        if (window.confirm(`Are you sure you want to delete account ${account.name}?`)) {
            csharpApi.deleteAccount(account);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        const accountToSave = {
            ...currentAccount,
            categoryId: currentAccount.categoryId === '' ? null : currentAccount.categoryId
        };
        if (accountToSave.id) {
            csharpApi.updateAccount(accountToSave);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentAccount({ ...currentAccount, [name]: value });
    };

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };
    
    const handleSaveSuccess = () => {
        setIsAddAccountOpen(false);
        fetchAccounts();
    };

    const getCategoryName = (categoryId) => {
        const category = categories.find(c => c.id === categoryId);
        return category ? category.name : 'N/A';
    };

    return (
        <div className="crud-container">
             {isAddAccountOpen && (
                <div className="modal">
                    <div className="modal-content-add">
                        <AddAccount 
                            onClose={() => setIsAddAccountOpen(false)} 
                            onSaveSuccess={handleSaveSuccess}
                        />
                    </div>
                </div>
            )}
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới tài khoản</button>
                </div>
                <div className="filter-section">
                    <label htmlFor="categoryFilter">Lọc theo danh mục:</label>
                    <select id="categoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
                        <option value="">Tất cả danh mục</option>
                        {accountCategories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên</th>
                                <th>UID</th>
                                <th>Avatar</th>
                                <th>Danh mục</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {accounts.map((account) => (
                                <tr key={account.id}>
                                    <td>{account.name}</td>
                                    <td>{account.uid}</td>
                                    <td>
                                        {account.avatar ? (
                                            <img src={account.avatar} alt="Avatar" style={{ width: '50px', height: '50px', borderRadius: '50%', objectFit: 'cover' }} />
                                        ) : (
                                            'N/A'
                                        )}
                                    </td>
                                    <td>{getCategoryName(account.categoryId)}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(account)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(account)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isEditModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>Sửa tài khoản</h2>
                            <button className="close-btn" onClick={() => setIsEditModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <div className="form-group">
                                <label>Tên:</label>
                                <input name="name" value={currentAccount?.name || ''} onChange={handleInputChange} required />
                            </div>
                            
                            <div className="form-group">
                                <label>UID:</label>
                                <input name="uid" value={currentAccount?.uid || ''} onChange={handleInputChange} required/>
                            </div>

                            <div className="form-group">
                                <label>Password:</label>
                                <input name="password" type="password" value={currentAccount?.password || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Email:</label>
                                <input name="email" value={currentAccount?.email || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Passmail:</label>
                                <input name="passmail" type="password" value={currentAccount?.passmail || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Phone:</label>
                                <input name="phone" value={currentAccount?.phone || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Mail Recovery:</label>
                                <input name="mailrecovery" value={currentAccount?.mailrecovery || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Private Key:</label>
                                <input name="privatekey" value={currentAccount?.privatekey || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group">
                                <label>Avatar URL:</label>
                                <input name="avatar" value={currentAccount?.avatar || ''} onChange={handleInputChange} />
                            </div>

                            <div className="form-group full-width">
                                <label>Cookies:</label>
                                <textarea name="cookies" value={currentAccount?.cookies || ''} onChange={handleInputChange} rows="4" />
                            </div>

                            <div className="form-group full-width">
                                <label>Danh mục:</label>
                                <select name="categoryId" value={currentAccount?.categoryId || ''} onChange={handleInputChange}>
                                    <option value="">-- Chọn danh mục --</option>
                                    {accountCategories.map(cat => (
                                        <option key={cat.id} value={cat.id}>{cat.name}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsEditModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AccountCrud;